using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using Microsoft.Win32;
using ChatClient.Models;
using ChatClient.Services;
using Microsoft.VisualBasic;

namespace ChatClient;

public partial class MainWindow : Window
{
    private readonly ResourceApiClient _resources = new();
    private readonly UserSettings _settings = new();
    private readonly LocalizationService _localization = new();
    private ChatServer? _server;
    private ChatClientConnection? _connection;
    private VirtualRoomConnection? _virtualRoom;
    private string _key = "";
    private CancellationTokenSource? _typingCts;
    private const long MaxAttachmentBytes = 10 * 1024 * 1024;

    public MainWindow() { InitializeComponent(); ApplyLanguage(); }

    private void Enter_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameBox.Text)) { System.Windows.MessageBox.Show("请输入您的称呼。", "提示"); return; }
        _settings.DisplayName = NameBox.Text.Trim(); GreetingText.Text = $"你好，{_settings.DisplayName}。请选择操作。";
        NetworkStatusText.Text = GetNetworkSummary(); WelcomePanel.Visibility = Visibility.Collapsed; HomePanel.Visibility = Visibility.Visible;
    }

    private async void WireGuard_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var keyPair = await WireGuardService.GenerateKeyPairAsync();
            var address = Interaction.InputBox("Virtual address (for example 10.77.0.2/24):", "WireGuard");
            var peerKey = Interaction.InputBox("Coordination server peer public key:", "WireGuard");
            var endpoint = Interaction.InputBox("Coordination server endpoint (host:port):", "WireGuard");
            if (string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(peerKey) || string.IsNullOrWhiteSpace(endpoint)) return;
            var config = WireGuardService.BuildConfig(keyPair.PrivateKey, address, 51820, new WireGuardPeer(peerKey, "10.77.0.0/24", endpoint));
            var dialog = new SaveFileDialog { Filter = "WireGuard configuration (*.conf)|*.conf", FileName = "ChatClient.conf" };
            if (dialog.ShowDialog() == true) { await File.WriteAllTextAsync(dialog.FileName, config); System.Windows.MessageBox.Show("WireGuard configuration exported. Import it into WireGuard and enable the tunnel.", "WireGuard"); }
        }
        catch (Exception ex) { ShowSafeError("WireGuard is unavailable", ex); }
    }

    private async void VirtualRoom_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var relay = Interaction.InputBox("公网中继 WebSocket 地址（例如 wss://your-domain.example/ws）：", "虚拟网络房间");
            if (string.IsNullOrWhiteSpace(relay)) return;
            var room = Interaction.InputBox("房间码；创建房间请留空后自动生成：", "虚拟网络房间");
            var creating = string.IsNullOrWhiteSpace(room);
            if (creating) room = Convert.ToHexString(RandomNumberGenerator.GetBytes(6));
            _key = creating ? Convert.ToHexString(RandomNumberGenerator.GetBytes(32)) : Interaction.InputBox("房间密钥：", "虚拟网络房间").Trim();
            if (_key.Length == 0) return;
            _virtualRoom = new VirtualRoomConnection(_key); _virtualRoom.PacketReceived += HandlePacket; _virtualRoom.MessageReceived += AddMessage;
            await _virtualRoom.ConnectAsync(relay.Trim(), room.Trim(), _settings.DisplayName);
            System.Windows.MessageBox.Show($"虚拟网络房间已连接。\n\n房间码：{room}\n房间密钥：{_key}\n\n请让其他用户使用相同中继地址、房间码和房间密钥加入。", "虚拟网络房间");
            OpenChat($"虚拟房间 · {room}");
        }
        catch (Exception ex) { ShowSafeError("虚拟网络连接失败", ex); if (_virtualRoom is not null) await _virtualRoom.DisposeAsync(); _virtualRoom = null; }
    }

    private string GetNetworkSummary()
    {
        var addresses = NetworkService.GetAddresses();
        if (addresses.Count == 0) return "未找到可用网络接口。";
        var lines = addresses.Take(4).Select(a => $"{a.Mode}: {a.Address}");
        return string.Join(Environment.NewLine, lines) + Environment.NewLine + (NetworkService.HasPublicIpv4() ? "已检测到公网 IPv4" : "抱歉，IPv4 网络下没找到公网地址") + (NetworkService.HasIpv6() ? "\n已检测到 IPv6" : "\n未检测到 IPv6，可尝试虚拟网络");
    }

    private async void CreateServer_Click(object sender, RoutedEventArgs e)
    {
        var addresses = NetworkService.GetAddresses();
        if (addresses.Count == 0) { System.Windows.MessageBox.Show("未找到可用于搭建服务器的网络。", "网络提示"); return; }
        var choices = string.Join(Environment.NewLine, addresses.Select((a, i) => $"{i + 1}. {a.Name} - {a.Address}"));
        var selected = Interaction.InputBox($"选择网络接口编号：\n{choices}", "选择搭建网络", "1");
        if (!int.TryParse(selected, out var index) || index < 1 || index > addresses.Count) return;
        var portText = Interaction.InputBox("请输入端口（默认 45678）：", "创建服务器", "45678");
        if (!int.TryParse(portText, out var port) || port is < 1024 or > 65535) port = 45678;
        _key = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)); var address = addresses[index - 1];
        _server = new ChatServer(address.Address, port, _key, _settings.DisplayName); _server.MessageReceived += AddMessage; _ = _server.StartAsync();
        System.Windows.MessageBox.Show($"服务器已开启\n\n地址：{address.Address}\n端口：{port}\n64 位密钥：{_key}\n\n请将这些信息安全地提供给加入者。", "服务器已启动");
        OpenChat($"服务器 · {address.Address}:{port}");
    }

    private async void JoinChat_Click(object sender, RoutedEventArgs e)
    {
        var host = Interaction.InputBox("服务器 IP 地址或主机名：", "加入聊天"); if (string.IsNullOrWhiteSpace(host)) return;
        var portText = Interaction.InputBox("服务器端口：", "加入聊天", "45678"); var key = Interaction.InputBox("64 位服务器密钥：", "加入聊天");
        if (!int.TryParse(portText, out var port) || string.IsNullOrWhiteSpace(key)) return;
        try { _key = key.Trim(); _connection = new ChatClientConnection(); _connection.MessageReceived += AddMessage; _connection.PacketReceived += HandlePacket; await _connection.ConnectAsync(host.Trim(), port, _settings.DisplayName, _key); OpenChat($"已连接 · {host}:{port}"); }
        catch (Exception ex) { System.Windows.MessageBox.Show($"连接失败：{ex.Message}", "加入失败"); }
    }

    private void OpenChat(string info) { HomePanel.Visibility = Visibility.Collapsed; ChatPanel.Visibility = Visibility.Visible; ConnectionInfo.Text = info; }
    private void AddMessage(string message) => Dispatcher.Invoke(() => MessagesList.Items.Add(message));
    private void HandlePacket(ChatPacket packet)
    {
        Dispatcher.Invoke(() =>
        {
            if (packet.Type == "typing") { TypingStatusText.Text = $"{packet.Name} 正在输入…"; return; }
            if (packet.Type == "typing-stop") { TypingStatusText.Text = ""; return; }
            if (packet.Type != "message") return;
            if (!string.IsNullOrWhiteSpace(packet.FileName))
            {
                var bytes = Convert.FromBase64String(packet.Data);
                var path = Path.Combine(Path.GetTempPath(), "ChatClient-" + Guid.NewGuid() + Path.GetExtension(packet.FileName));
                File.WriteAllBytes(path, bytes);
                var panel = new StackPanel(); panel.Children.Add(new TextBlock { Text = $"{packet.Name}: {packet.FileName}" });
                if (packet.MediaType.StartsWith("image/")) panel.Children.Add(new System.Windows.Controls.Image { Source = new BitmapImage(new Uri(path)), Width = 240, MaxHeight = 180, Stretch = Stretch.Uniform });
                else if (packet.MediaType.StartsWith("video/")) panel.Children.Add(new MediaElement { Source = new Uri(path), Width = 320, Height = 200, LoadedBehavior = MediaState.Manual, UnloadedBehavior = MediaState.Stop });
                MessagesList.Items.Add(panel);
            }
            else MessagesList.Items.Add($"{packet.Name}: {packet.Text}");
            TypingStatusText.Text = "";
        });
    }
    private async void MessageBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_key) || MessageBox.Text.Length == 0) return;
        _typingCts?.Cancel(); _typingCts = new CancellationTokenSource();
        var token = _typingCts.Token;
        await SendPacketAsync(new ChatPacket { Type = "typing", Name = _settings.DisplayName, Key = _key });
        try { await Task.Delay(1200, token); await SendPacketAsync(new ChatPacket { Type = "typing-stop", Name = _settings.DisplayName, Key = _key }); } catch (OperationCanceledException) { }
    }
    private async void Attach_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog { Filter = "媒体和文件|*.png;*.jpg;*.jpeg;*.gif;*.mp4;*.webm;*.mov;*.*" };
        if (dialog.ShowDialog() != true) return;
        var info = new FileInfo(dialog.FileName); if (info.Length > MaxAttachmentBytes) { System.Windows.MessageBox.Show("文件不能超过 10 MB。", "发送失败"); return; }
        var data = Convert.ToBase64String(await File.ReadAllBytesAsync(dialog.FileName));
        var mediaType = dialog.FileName.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ? "image/gif" : dialog.FileName.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase) ? "video/mp4" : dialog.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) || dialog.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || dialog.FileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ? "image/*" : "application/octet-stream";
        var packet = new ChatPacket { Type = "message", Name = _settings.DisplayName, Key = _key, FileName = info.Name, MediaType = mediaType, Data = data };
        await SendPacketAsync(packet);
        if (_server is not null) HandlePacket(packet);
    }
    private async Task SendPacketAsync(ChatPacket packet)
    { if (_connection is not null) await _connection.SendAsync(packet); else if (_virtualRoom is not null) await _virtualRoom.SendAsync(packet); else if (_server is not null) await _server.PublishAsync(packet); }
    private async void Send_Click(object sender, RoutedEventArgs e) { try { await SendMessageAsync(); } catch (Exception ex) { ShowSafeError("发送失败", ex); } }
    private async void MessageBox_KeyDown(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) { e.Handled = true; try { await SendMessageAsync(); } catch (Exception ex) { ShowSafeError("发送失败", ex); } } }
    private async Task SendMessageAsync()
    {
        var text = MessageBox.Text.Trim(); if (text.Length == 0) return; MessageBox.Clear();
        var packet = new ChatPacket { Name = _settings.DisplayName, Key = _key, Encrypted = _settings.EncryptMessages };
        if (_settings.EncryptMessages) { var encrypted = CryptoService.Encrypt(text, _key); packet.Text = encrypted.Cipher; packet.Nonce = encrypted.Nonce; packet.Tag = encrypted.Tag; }
        else packet.Text = text;
        if (_connection is not null) await _connection.SendAsync(packet);
        else if (_virtualRoom is not null) await _virtualRoom.SendAsync(packet);
        else if (_server is not null) { AddMessage($"{_settings.DisplayName}: {text}"); await _server.PublishAsync(packet); }
        else AddMessage($"{_settings.DisplayName}: {text}");
    }

    private async void Resource_Click(object sender, RoutedEventArgs e)
    { var name = Interaction.InputBox("请输入资源名称（例如 XXX）：", "兑换信息"); if (string.IsNullOrWhiteSpace(name)) return; var result = await _resources.GetAsync(name); System.Windows.MessageBox.Show(result, "兑换结果"); }
    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow(_settings, _localization, ApplyTheme) { Owner = this };
        settingsWindow.ShowDialog();
        ApplyLanguage();
    }
    private void ApplyLanguage()
    {
        WelcomeTitle.Text = _localization.Get("welcome"); WelcomeQuestion.Text = _localization.Get("askName"); EnterButton.Content = _localization.Get("enter");
        ResourceButton.Content = _localization.Get("resource"); SettingsButton.Content = _localization.Get("settings"); AboutButton.Content = _localization.Get("about");
        ConnectionTitle.Text = _localization.Get("connection"); CreateButton.Content = _localization.Get("create"); JoinButton.Content = _localization.Get("join");
        VirtualRoomButton.Content = _localization.Get("virtual"); WireGuardButton.Content = _localization.Get("wireguard"); ReadyTitle.Text = _localization.Get("ready"); ChatTitle.Text = _localization.Get("chat"); SendButton.Content = _localization.Get("send"); LeaveButton.Content = _localization.IsEnglish ? "Leave Room" : "退出房间";
        Title = _localization.IsEnglish ? "ChatClient" : "ChatClient（中文）";
    }
    private void ApplyTheme(bool dark)
    { _settings.DarkMode = dark; Resources["WindowBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(dark ? "#111827" : "#F5F7FB")!); Resources["PanelBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(dark ? "#1F2937" : "#FFFFFF")!); Resources["TextBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(dark ? "#F9FAFB" : "#182230")!); Resources["MutedBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(dark ? "#CBD5E1" : "#667085")!); }
    private void About_Click(object sender, RoutedEventArgs e) => System.Windows.MessageBox.Show("ChatClient v1.1.0\n作者：Creaddinscart Team\n版权：MIT License\n\n支持 IPv4、IPv6、局域网与虚拟网络。", "关于 ChatClient");
    private async void LeaveRoom_Click(object sender, RoutedEventArgs e)
    {
        if (_connection is not null) { await _connection.DisposeAsync(); _connection = null; }
        if (_virtualRoom is not null) { await _virtualRoom.DisposeAsync(); _virtualRoom = null; }
        if (_server is not null) { await _server.DisposeAsync(); _server = null; }
        MessagesList.Items.Clear(); ChatPanel.Visibility = Visibility.Collapsed; HomePanel.Visibility = Visibility.Visible; NetworkStatusText.Text = GetNetworkSummary();
    }
    private static void ShowSafeError(string title, Exception ex) => System.Windows.MessageBox.Show($"{title}：{ex.Message}", title);
    protected override async void OnClosed(EventArgs e) { if (_connection is not null) await _connection.DisposeAsync(); if (_virtualRoom is not null) await _virtualRoom.DisposeAsync(); if (_server is not null) await _server.DisposeAsync(); base.OnClosed(e); }
}
