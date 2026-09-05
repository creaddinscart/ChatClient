using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using ChatClient.Models;

namespace ChatClient.Services;

public static class NetworkService
{
    public static IReadOnlyList<NetworkAddress> GetAddresses()
    {
        var result = new List<NetworkAddress>();
        foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (adapter.OperationalStatus != OperationalStatus.Up || adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;
            var virtualAdapter = adapter.Description.Contains("virtual", StringComparison.OrdinalIgnoreCase)
                || adapter.Description.Contains("wireguard", StringComparison.OrdinalIgnoreCase)
                || adapter.Description.Contains("zerotier", StringComparison.OrdinalIgnoreCase)
                || adapter.Description.Contains("tailscale", StringComparison.OrdinalIgnoreCase);
            foreach (var address in adapter.GetIPProperties().UnicastAddresses)
            {
                if (address.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(address.Address))
                    result.Add(new NetworkAddress($"{adapter.Name} · IPv4", virtualAdapter ? NetworkMode.VirtualNetwork : NetworkMode.IPv4, address.Address, virtualAdapter));
                if (address.Address.AddressFamily == AddressFamily.InterNetworkV6 && !address.Address.IsIPv6LinkLocal && !IPAddress.IsLoopback(address.Address))
                    result.Add(new NetworkAddress($"{adapter.Name} · IPv6", virtualAdapter ? NetworkMode.VirtualNetwork : NetworkMode.IPv6, address.Address, virtualAdapter));
            }
        }
        return result;
    }

    public static bool HasPublicIpv4() => GetAddresses().Any(a => a.Mode == NetworkMode.IPv4 && !IsPrivate(a.Address));
    public static bool HasIpv6() => GetAddresses().Any(a => a.Mode == NetworkMode.IPv6);

    private static bool IsPrivate(IPAddress ip)
    {
        var b = ip.GetAddressBytes();
        return b[0] == 10 || (b[0] == 172 && b[1] >= 16 && b[1] <= 31) || (b[0] == 192 && b[1] == 168);
    }
}
