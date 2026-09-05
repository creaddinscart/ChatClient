using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace ChatClient.Services;

public sealed record WireGuardKeyPair(string PrivateKey, string PublicKey);
public sealed record WireGuardPeer(string PublicKey, string AllowedIps, string Endpoint);

public static class WireGuardService
{
    public static async Task<WireGuardKeyPair> GenerateKeyPairAsync(CancellationToken token = default)
    {
        var privateKey = await RunWgAsync("genkey", null, token);
        var publicKey = await RunWgAsync("pubkey", privateKey, token);
        return new WireGuardKeyPair(privateKey.Trim(), publicKey.Trim());
    }

    public static string BuildConfig(string privateKey, string address, int listenPort, WireGuardPeer peer, string dns = "1.1.1.1")
    {
        if (!IPAddressParser.TryParseCidr(address)) throw new ArgumentException("虚拟 IP 地址格式无效。", nameof(address));
        return $"[Interface]\nPrivateKey = {privateKey.Trim()}\nAddress = {address}\nListenPort = {listenPort}\nDNS = {dns}\n\n[Peer]\nPublicKey = {peer.PublicKey.Trim()}\nAllowedIPs = {peer.AllowedIps.Trim()}\nEndpoint = {peer.Endpoint.Trim()}\nPersistentKeepalive = 25\n";
    }

    private static async Task<string> RunWgAsync(string arguments, string? input, CancellationToken token)
    {
        var start = new ProcessStartInfo("wg.exe", arguments) { RedirectStandardInput = true, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false, CreateNoWindow = true };
        using var process = Process.Start(start) ?? throw new InvalidOperationException("无法启动 wg.exe，请安装 WireGuard。");
        if (input is not null) { await process.StandardInput.WriteAsync(input); await process.StandardInput.FlushAsync(token); process.StandardInput.Close(); }
        var output = await process.StandardOutput.ReadToEndAsync(token); var error = await process.StandardError.ReadToEndAsync(token); await process.WaitForExitAsync(token);
        if (process.ExitCode != 0) throw new InvalidOperationException(string.IsNullOrWhiteSpace(error) ? "WireGuard 工具执行失败。" : error.Trim());
        return output;
    }

    private static class IPAddressParser
    {
        public static bool TryParseCidr(string value)
        { var parts = value.Split('/'); return parts.Length == 2 && System.Net.IPAddress.TryParse(parts[0], out _) && int.TryParse(parts[1], out var prefix) && prefix is >= 0 and <= 128; }
    }
}
