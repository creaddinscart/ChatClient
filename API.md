# ChatClient 接口说明

## Language / 语言

The default language is `en-US`. Settings can switch to `zh-CN`; the selection applies immediately to the main window.

## 资源兑换接口

客户端请求：

`GET https://shit.pub/s/developer/Client/ChatClient/API/s/{resource}/txt.txt`

其中 `{resource}` 只允许字母、数字、短横线、下划线和点。客户端使用 HTTPS、15 秒超时，每 60 秒最多发起一次兑换请求。

- HTTP 2xx 且正文非空：显示返回文本。
- 其他状态码或空正文：显示“抱歉，您搜索的内容貌似没找到”。
- 网络失败：显示可读错误信息，不显示敏感异常堆栈。

## 聊天协议

TCP 连接上的每行是一个 UTF-8 JSON 对象：

- `Type`：`join` 或 `message`
- `Name`：显示名称
- `Text`：明文或 Base64 密文
- `Key`：服务器 64 位十六进制密钥
- `Encrypted`：是否 AES-GCM 加密
- `Nonce`、`Tag`：加密消息的 Base64 参数

默认端口为 45678，可在创建服务器时修改。服务器只广播密钥匹配的消息。

## 虚拟网络房间中继

客户端连接用户填写的 `ws://` 或 `wss://` 地址，并附加查询参数 `room` 和 `name`。连接后以 WebSocket 文本消息传输与 TCP 模式相同的 `ChatPacket` JSON。中继服务应将同一 `room` 内的消息转发给其他连接，并建议只转发 `Key` 匹配的消息。

中继服务至少需要实现：WebSocket Upgrade、按 `room` 分组、文本 JSON 广播、断线清理和消息大小限制。ChatClient 本身不包含公网中继服务器，因此必须先部署或填写一个真实可用的中继地址。

## WireGuard configuration / WireGuard 配置

The Real Virtual LAN button invokes `wg.exe genkey` and `wg.exe pubkey`, then exports a standard WireGuard `.conf` file. The peer must provide:

- a server peer public key;
- a public endpoint in `host:port` form;
- a shared virtual CIDR such as `10.77.0.0/24`.

客户端只负责生成密钥和配置文件。安装 WireGuard/Wintun、配置协调节点并导入配置后，系统程序才能访问该虚拟局域网。
