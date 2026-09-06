Release 1.4.0.0 - Radmin VPN integration and cross-platform updates

Summary:
- Added Radmin VPN integration (detection and quick-launch/download) with Home UI buttons on both Windows WPF and Avalonia cross-platform clients.
- Bumped project versions to 1.4.0.0 and updated documentation.
- Radmin VPN is not bundled; the app can open the official download page and launch a locally installed Radmin client.
- UI: Home page now features Radmin VPN quick actions and descriptive text explaining that Radmin provides a friendly free virtual LAN for multiplayer sessions.

Notes:
- Radmin integration currently detects installation and launches the Radmin application on Windows. If not found, the download page opens in the default browser.
- For licensing reasons, Radmin binaries are not distributed with ChatClient.
- Future work: deeper Radmin automation (create/join network via API/CLI) if supported by Radmin and requested.

Published artifacts:
- Binaries will be produced by running publish scripts after building with version 1.4.0.0.

Suggested manual tests:
1. On Windows with Radmin installed: open ChatClient -> Home -> click "Radmin VPN" -> verify app launches and message shown.
2. On Windows without Radmin: click "Radmin VPN" -> verify browser opens to the official download page.
3. Cross-platform: click Radmin VPN in Avalonia client -> verify messages appended and browser opens on non-Windows machines.

Published file locations:
- Windows: publish\1.4.0.0\ChatClient.exe
- Cross-platform: publish-crossplatform\1.4.0.0\{rid}

If further integration is required (e.g., automated install or CLI control), provide details and we can extend the service accordingly.
