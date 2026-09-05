using System.Net.WebSockets;
using System.IO;
using System.Text;
using System.Text.Json;
using ChatClient.Models;

namespace ChatClient.Services;

public sealed class VirtualRoomConnection : IAsyncDisposable
{
    private readonly ClientWebSocket _socket = new();
    private readonly string _key;
    public event Action<string>? MessageReceived;
    public event Action<ChatPacket>? PacketReceived;

    public VirtualRoomConnection(string key) => _key = key;

    public async Task ConnectAsync(string relayUrl, string roomCode, string name, CancellationToken token = default)
    {
        if (!Uri.TryCreate(relayUrl, UriKind.Absolute, out var baseUri) || baseUri.Scheme is not ("ws" or "wss"))
            throw new ArgumentException("中继地址必须是 ws:// 或 wss:// 地址。", nameof(relayUrl));
        if (string.IsNullOrWhiteSpace(roomCode)) throw new ArgumentException("房间码不能为空。", nameof(roomCode));
        var uri = new UriBuilder(baseUri) { Query = $"room={Uri.EscapeDataString(roomCode)}&name={Uri.EscapeDataString(name)}" }.Uri;
        await _socket.ConnectAsync(uri, token);
        await SendAsync(new ChatPacket { Type = "join", Name = name, Key = _key });
        _ = ReceiveLoopAsync(token);
    }

    public async Task SendAsync(ChatPacket packet, CancellationToken token = default)
    {
        if (_socket.State != WebSocketState.Open) throw new InvalidOperationException("虚拟网络房间尚未连接。");
        var data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(packet));
        await _socket.SendAsync(data, WebSocketMessageType.Text, true, token);
    }

    private async Task ReceiveLoopAsync(CancellationToken token)
    {
        var buffer = new byte[64 * 1024];
        try
        {
            while (_socket.State == WebSocketState.Open && !token.IsCancellationRequested)
            {
                using var content = new MemoryStream(); WebSocketReceiveResult result;
                do { result = await _socket.ReceiveAsync(buffer, token); if (result.MessageType == WebSocketMessageType.Close) return; content.Write(buffer, 0, result.Count); } while (!result.EndOfMessage);
                ChatPacket? packet; try { packet = JsonSerializer.Deserialize<ChatPacket>(content.ToArray()); } catch (JsonException) { continue; }
                if (packet is null) continue;
                try { if (packet.Type == "message" && packet.Encrypted) packet.Text = CryptoService.Decrypt(packet.Text, _key, packet.Nonce, packet.Tag); PacketReceived?.Invoke(packet); }
                catch (System.Security.Cryptography.CryptographicException) { MessageReceived?.Invoke("收到无法解密的虚拟房间消息，已忽略。"); }
            }
        }
        catch (OperationCanceledException) { }
        catch (WebSocketException) { MessageReceived?.Invoke("虚拟网络房间连接已断开。"); }
    }

    public async ValueTask DisposeAsync()
    {
        if (_socket.State == WebSocketState.Open) try { await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closed", CancellationToken.None); } catch { }
        _socket.Dispose();
    }
}
