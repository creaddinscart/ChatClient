using System.Net;
using System.Text.Json.Serialization;

namespace ChatClient.Models;

public enum NetworkMode { IPv4, IPv6, VirtualNetwork }

public sealed class ChatPacket
{
    public string Type { get; set; } = "message";
    public string Name { get; set; } = "";
    public string Text { get; set; } = "";
    public string Key { get; set; } = "";
    public bool Encrypted { get; set; }
    public string Nonce { get; set; } = "";
    public string Tag { get; set; } = "";
    public string FileName { get; set; } = "";
    public string MediaType { get; set; } = "";
    public string Data { get; set; } = "";
}

public sealed record NetworkAddress(string Name, NetworkMode Mode, IPAddress Address, bool IsVirtual);

public sealed class UserSettings
{
    public string DisplayName { get; set; } = "";
    public bool DarkMode { get; set; }
    public string ChatColor { get; set; } = "#2563EB";
    public bool EncryptMessages { get; set; } = true;
}
