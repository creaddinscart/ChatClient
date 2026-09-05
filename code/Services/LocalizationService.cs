namespace ChatClient.Services;

public sealed class LocalizationService
{
    public bool IsEnglish { get; private set; } = true;
    public event Action? Changed;
    private readonly Dictionary<string, (string Zh, string En)> _text = new()
    {
        ["welcome"] = ("欢迎来到 CC", "Welcome to CC"), ["askName"] = ("请问我们如何称呼您？", "What should we call you?"), ["enter"] = ("进入 ChatClient", "Enter ChatClient"),
        ["create"] = ("创建服务器", "Create Server"), ["join"] = ("加入聊天", "Join Chat"), ["virtual"] = ("虚拟网络房间", "Virtual Network Room"), ["wireguard"] = ("真实虚拟局域网", "Real Virtual LAN"),
        ["settings"] = ("设置", "Settings"), ["about"] = ("关于", "About"), ["resource"] = ("兑换信息", "Redeem Resource"), ["connection"] = ("连接中心", "Connection Center"),
        ["ready"] = ("准备开始", "Ready to start"), ["chat"] = ("聊天房间", "Chat Room"), ["send"] = ("发送", "Send"), ["language"] = ("语言：中文 / English", "Language: 中文 / English"),
        ["languageChanged"] = ("语言已切换为中文。", "Language changed to English."), ["wireguardInfo"] = ("需要安装 WireGuard/Wintun，并配置协调服务器。", "Requires WireGuard/Wintun and a coordination server."),
        ["cancel"] = ("取消", "Cancel"), ["ok"] = ("确定", "OK")
    };
    public string Get(string key) => _text.TryGetValue(key, out var value) ? (IsEnglish ? value.En : value.Zh) : key;
    public void SetLanguage(bool english) { IsEnglish = english; Changed?.Invoke(); }
}
