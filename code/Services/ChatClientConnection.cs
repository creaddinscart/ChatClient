using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ChatClient.Models;

namespace ChatClient.Services;

public sealed class ChatClientConnection : IAsyncDisposable
{
    private TcpClient? _client; private StreamWriter? _writer;
    private string _key = "";
    public event Action<string>? MessageReceived;
    public event Action<ChatPacket>? PacketReceived;
    public async Task ConnectAsync(string host, int port, string name, string key, CancellationToken token = default)
    {
        _key = key; _client = new TcpClient(); await _client.ConnectAsync(host, port, token); var stream = _client.GetStream(); _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true }; var reader = new StreamReader(stream, Encoding.UTF8);
        await _writer.WriteLineAsync(JsonSerializer.Serialize(new ChatPacket { Type = "join", Name = name, Key = key }));
        var responseLine = await reader.ReadLineAsync(token); var response = responseLine is null ? null : JsonSerializer.Deserialize<ChatPacket>(responseLine);
        if (response?.Type != "auth-ok") throw new UnauthorizedAccessException("服务器密钥错误，加入被拒绝。");
        _ = ReadAsync(reader, token);
    }
    public async Task SendAsync(ChatPacket packet) { if (_writer is null) return; try { await _writer.WriteLineAsync(JsonSerializer.Serialize(packet)); } catch (IOException) { MessageReceived?.Invoke("连接已断开，消息未发送。"); } catch (ObjectDisposedException) { MessageReceived?.Invoke("连接已关闭，消息未发送。"); } }
    private async Task ReadAsync(StreamReader reader, CancellationToken token)
    { try { string? line; while ((line = await reader.ReadLineAsync(token)) is not null) { ChatPacket? p; try { p = JsonSerializer.Deserialize<ChatPacket>(line); } catch (JsonException) { continue; } if (p is null || p.Type is "auth-ok" or "auth-failed") continue; if (p.Type == "message") { try { if (p.Encrypted) p.Text = CryptoService.Decrypt(p.Text, _key, p.Nonce, p.Tag); } catch (CryptographicException) { MessageReceived?.Invoke("收到无法解密的消息，已忽略。"); continue; } } PacketReceived?.Invoke(p); } } catch (OperationCanceledException) { } catch (IOException) { MessageReceived?.Invoke("聊天连接已断开。"); } catch (ObjectDisposedException) { } }
    public ValueTask DisposeAsync() { _client?.Dispose(); return ValueTask.CompletedTask; }
}
