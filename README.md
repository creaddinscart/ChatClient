# ChatClient

Current release: **v1.4.0.0**.

## Quick start

1. Launch the app, enter your display name, and select **Enter ChatClient**.
2. Select **Create Server**, choose a local network interface, and record the address, port, and server key.
3. On another device, or in a second client on the same computer, select **Join Chat** and enter the server details.
4. Allow the selected port through Windows Firewall. Public IPv4 connections may also require router port forwarding.
5. For a relay room, select **Virtual Network Room**, enter a WebSocket relay address, and share the generated room code and key.
6. For a real virtual LAN, select **Real Virtual LAN**, export the WireGuard configuration, import it into WireGuard, and enable the tunnel.

## Features

- English-only user interface and notifications.
- Light and dark themes with a custom chat accent color.
- IPv4, IPv6, LAN, virtual-room, and WireGuard networking options.
- TCP server/client chat with strict client protocol-version matching.
- Optional AES-GCM message encryption for password-protected servers.
- File, image, GIF, and video attachments up to 100 MB with preview and download.
- Server-owner moderation: ban by username, IP address, or both.
- Usernames must be unique within each server, including the server owner name.
- Online-user count displayed in the active server chat panel.
- Full session chat-history export to a `.txt` file.
- Typing indicators visible to other participants, including password-free rooms.
- Enter sends a message; Shift+Enter inserts a new line.
- Resource redemption with a one-minute request throttle.

## Server and security notes

The server key is generated for the current session and must be shared only with intended participants. IP bans are based on the address observed by the server and may be affected by NAT or relay topologies.

The application does not include a VPN driver or automatically modify router settings. WireGuard mode requires WireGuard for Windows, Wintun, and a compatible coordination or relay server.

## Build and publish

Windows desktop build requirements: Windows, .NET 8 SDK or later, and the WPF workload.
Cross-platform build requirements: .NET 8 SDK or later. The Avalonia client can be published for Windows, Linux, and macOS.

```powershell
dotnet build ChatClient.csproj
powershell.exe -ExecutionPolicy Bypass -File .\publish.ps1 -Version 1.3.0.0
powershell.exe -ExecutionPolicy Bypass -File .\publish-crossplatform.ps1 -Version 1.3.0.0
```

The self-contained single-file executable is generated at:

`publish\1.3.0.0\ChatClient.exe`

The cross-platform single-file executables are generated under
`publish-crossplatform\1.3.0.0\` in RID-specific folders for Windows x86/x64/ARM64,
Linux x86/ARM/ARM64 and musl x64/ARM64, and macOS x64/ARM64.

The publish script uses `--no-restore` after dependencies are already restored to reduce repeated publish time.

## Release notes

See [RELEASE_NOTES_1.3.0.0.md](RELEASE_NOTES_1.3.0.0.md) for the current release and [RELEASE_NOTES_1.2.7.md](RELEASE_NOTES_1.2.7.md) for the previous release.
