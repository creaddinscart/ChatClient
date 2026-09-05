using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ChatClient.Models;

namespace ChatClient.Services;

public sealed class ChatServer : IAsyncDisposable
{
    private readonly TcpListener _listener;
    private readonly List<(TcpClient Client, StreamWriter Writer)> _clients = new();
    private CancellationTokenSource? _cts;
    public string Key { get; }
    public int Port { get; }
    public string OwnerName { get; }
    public event Action<string>? MessageReceived;

    public ChatServer(IPAddress address, int port, string key, string ownerName = "") { _listener = new TcpListener(address, port); Port = port; Key = key; OwnerName = ownerName; }
    public async Task StartAsync()
    {
        _cts = new CancellationTokenSource(); _listener.Start();
        while (!_cts.IsCancellationRequested)
        {
            try { var client = await _listener.AcceptTcpClientAsync(_cts.Token); _ = HandleAsync(client); }
            catch (OperationCanceledException) { break; }
            catch (SocketException) { if (!_cts.IsCancellationRequested) await Task.Delay(250); }
        }
    }
    private async Task HandleAsync(TcpClient client)
    {
        Stream? stream = null;
        StreamReader? reader = null;
        StreamWriter? writer = null;
        try
        {
            stream = client.GetStream();
            reader = new StreamReader(stream, Encoding.UTF8);
            writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            string? line = await reader.ReadLineAsync();
            ChatPacket? join;
            try { join = line is null ? null : JsonSerializer.Deserialize<ChatPacket>(line); } catch (JsonException) { join = null; }
            if (join is null || join.Type != "join" || join.Key != Key || string.IsNullOrWhiteSpace(join.Name))
            {
                await writer.WriteLineAsync(JsonSerializer.Serialize(new ChatPacket { Type = "auth-failed" }));
                return;
            }

            await writer.WriteLineAsync(JsonSerializer.Serialize(new ChatPacket { Type = "auth-ok" }));

            // Add to client list so BroadcastAsync can write to this writer
            lock (_clients) _clients.Add((client, writer));

            while ((line = await reader.ReadLineAsync()) is not null)
            {
                ChatPacket? packet;
                try { packet = JsonSerializer.Deserialize<ChatPacket>(line); } catch (JsonException) { continue; }
                if (packet is null || packet.Key != Key || string.IsNullOrWhiteSpace(packet.Name) || packet.Type == "join") continue;
                if (packet.Type == "message")
                {
                    string text;
                    try { text = packet.Encrypted ? CryptoService.Decrypt(packet.Text, Key, packet.Nonce, packet.Tag) : packet.Text; } catch (CryptographicException) { continue; }
                    if (!string.Equals(packet.Name, OwnerName, StringComparison.Ordinal)) MessageReceived?.Invoke($"{packet.Name}: {text}");
                }
                await BroadcastAsync(line);
            }
        }
        catch (IOException) { }
        catch (ObjectDisposedException) { }
        catch (SocketException) { }
        finally
        {
            try
            {
                // remove the tuple for this client
                lock (_clients)
                {
                    var idx = _clients.FindIndex(t => t.Client == client);
                    if (idx >= 0) _clients.RemoveAt(idx);
                }
            }
            catch { }

            try { writer?.Dispose(); } catch { }
            try { reader?.Dispose(); } catch { }
            try { client.Dispose(); } catch { }
        }
    }
    private async Task BroadcastAsync(string line)
    {
        (TcpClient Client, StreamWriter Writer)[] clients;
        lock (_clients) clients = _clients.ToArray();
        foreach (var c in clients)
        {
            try { await c.Writer.WriteLineAsync(line); }
            catch { /* ignore per-client errors */ }
        }
    }
    public async Task PublishAsync(ChatPacket packet)
    {
        if (packet.Key != Key) throw new UnauthorizedAccessException("服务器密钥不匹配。");
        await BroadcastAsync(JsonSerializer.Serialize(packet));
    }
    public async ValueTask DisposeAsync()
    {
        _cts?.Cancel();
        _listener.Stop();
        (TcpClient Client, StreamWriter Writer)[] clients;
        lock (_clients) clients = _clients.ToArray();
        foreach (var c in clients)
        {
            try { c.Writer.Dispose(); } catch { }
            try { c.Client.Dispose(); } catch { }
        }
        await Task.CompletedTask;
    }
}
