# ChatClient

ChatClient 是由 **Creaddinscart Team** 发布的 Windows WPF 聊天客户端，版本 **v1.1.0**，采用 MIT License。

> The application starts in English. Use **Settings** to switch between English and Chinese.

## 快速使用

1. 启动后输入称呼，点击“进入 ChatClient”。
2. 点击“创建服务器”，选择本机 IPv4、IPv6 或虚拟网络网卡，记录地址、端口和 64 位密钥。
3. 在另一台电脑或同一台电脑的第二个客户端点击“加入聊天”，填写服务器信息。
4. 服务器端口必须允许 Windows 防火墙通过；公网 IPv4 通常还需要路由器端口转发。
5. 使用虚拟网络时，双方必须先加入同一个虚拟网络，随后选择虚拟网卡地址连接。
6. 如不想安装虚拟网卡，点击“虚拟网络房间”：创建者填写中继 WebSocket 地址并将自动生成的房间码、房间密钥发给其他用户；加入者填写相同中继地址、房间码和房间密钥。
7. 如需真正的虚拟局域网，点击 **Real Virtual LAN**。安装 WireGuard 后导出配置，并导入 WireGuard 客户端启用隧道。

## 功能

- 白色官方模式、黑色官方模式，以及自定义聊天主色。
- IPv4、IPv6 和虚拟网卡检测。
- TCP 局域网/公网直连；同一台电脑可同时运行服务端和客户端。
- 虚拟网络房间模式：通过公网 WebSocket 中继连接，不安装 VPN 驱动；中继地址需要由项目部署者提供。
- 服务器生成 64 个十六进制字符的密钥，加入者必须提供正确密钥。
- 聊天消息加密可在设置中自行选择。启用后使用 AES-GCM；密钥只用于当前会话，不上传。
- 左上角“兑换信息”按名称请求 `https://shit.pub/s/developer/Client/ChatClient/API/s/{名称}/txt.txt`，每分钟最多请求一次。

## 限制与安全

本版本不内置 VPN 驱动，也不会自动修改路由器。虚拟网络房间需要一个兼容 `API.md` 协议的公网 WebSocket 中继服务；客户端不能凭空提供公网中继。请不要在不可信环境中公开服务器密钥或房间密钥。

## Real Virtual LAN / 真实虚拟局域网

该模式使用 WireGuard/Wintun 创建系统级虚拟网卡，使加入者获得同一虚拟网段中的 IP。客户端负责生成密钥和导出 `.conf`；仍需要：

- 安装 WireGuard for Windows（包含 Wintun）并允许驱动权限；
- 一个具有公网 Endpoint 的 WireGuard 协调/中继服务器；
- 服务端 Peer 公钥、Endpoint 和虚拟网段配置。

ChatClient 不会伪造隧道状态，也不会在缺少驱动或协调服务器时显示“已连接”。

### English

The application starts in English and can be switched to Chinese from Settings. **Real Virtual LAN** uses WireGuard/Wintun and exports a configuration file. Install WireGuard for Windows, configure a public coordination/relay peer, import the generated `.conf`, and enable the tunnel. The client does not claim that a tunnel is connected when the driver or coordination server is unavailable.

## 构建与发布

需要 .NET 8 SDK 或更高版本、Windows 和 Visual Studio WPF 工作负载。运行：

```powershell
./publish.ps1
```

发布文件位于 `publish/win-x64/ChatClient.exe`，为 self-contained 单文件 EXE。
