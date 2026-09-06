# ChatClient

ChatClient is a Windows WPF chat client released by the **Creaddinscart Team**, version **v1.1.0**, licensed under the MIT License.

> The application starts in English. Use **Settings** to switch between English and Chinese.

## Quick Start

1. After launching, enter a name and click "Enter ChatClient".

2. Click "Create Server", select your local IPv4, IPv6, or virtual network adapter, and record the address, port, and 64-bit key.

3. On another computer or a second client on the same computer, click "Join Chat" and fill in the server information.

4. The server port must be allowed through the Windows Firewall; public IPv4 usually requires router port forwarding.

5. When using a virtual network, both parties must first join the same virtual network, then select the virtual network adapter address to connect.

6. If you do not want to install a virtual network adapter, click "Virtual Network Room": The creator enters the relay WebSocket address and sends the automatically generated room code and room key to other users; joiners enter the same relay address, room code, and room key.

7. For a true virtual LAN, click **Real Virtual LAN**. After installing WireGuard, export the configuration and import it into the WireGuard client to enable tunneling.

## Features

- White official mode, black official mode, and customizable chat main color.

- IPv4, IPv6, and virtual network adapter detection.

- Direct TCP LAN/Public network connection; the server and client can run simultaneously on the same computer.

- Virtual Network Room mode: Connects via a public WebSocket relay, without installing a VPN driver; the relay address needs to be provided by the project deployer.

- The server generates a 64-character hexadecimal key; joiners must provide the correct key.

- Chat message encryption can be selected in the settings. When enabled, AES-GCM is used; the key is only used in the current session and is not uploaded.

- The "Redeem Information" in the top left corner requests `https://shit.pub/s/developer/Client/ChatClient/API/s/{name}/txt.txt` by name, with a maximum request of once per minute.

## Limitations and Security

This version does not include a built-in VPN driver and will not automatically modify the router. Virtual network rooms require a public WebSocket relay service compatible with the `API.md` protocol; clients cannot provide a public relay out of thin air. Do not expose server keys or room keys in untrusted environments.

## Real Virtual LAN

This mode uses WireGuard/Wintun to create a system-level virtual network adapter, allowing participants to obtain IPs within the same virtual network segment. The client is responsible for generating the key and exporting the `.conf` file; the following are still required:

- Install WireGuard for Windows (including Wintun) and allow driver permissions;

- A WireGuard coordination/relay server with a public endpoint;

- Server-side peer public key, endpoint, and virtual network segment configuration.

ChatClient does not spoof tunnel status and will not display "connected" when the driver or coordination server is missing.

### English

The application starts in English and can be switched to Chinese from Settings. **Real Virtual LAN** uses WireGuard/Wintun and exports a configuration file. Install WireGuard for Windows, configure a public coordination/relay peer, import the generated `.conf` file, and enable the tunnel. The client does not claim that a tunnel is connected when the driver or coordination server is unavailable.

## Building and Deployment

Requires .NET 8 SDK or later, Windows, and a Visual Studio WPF workload. Run:

```powershell

./publish.ps1

```
The published file is located at `publish/win-x64/ChatClient.exe`, and is a self-contained single-file EXE.
