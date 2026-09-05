using System.Windows;
using System.Windows.Media;
using ChatClient.Models;
using ChatClient.Services;

namespace ChatClient;

public partial class SettingsWindow : Window
{
    private readonly UserSettings _settings;
    private readonly LocalizationService _localization;
    private readonly Action<bool> _applyTheme;

    public SettingsWindow(UserSettings settings, LocalizationService localization, Action<bool> applyTheme)
    {
        InitializeComponent(); _settings = settings; _localization = localization; _applyTheme = applyTheme;
        ColorBox.Text = settings.ChatColor; EncryptionBox.IsChecked = settings.EncryptMessages; ApplyLanguage();
    }

    private void ApplyLanguage()
    {
        var english = _localization.IsEnglish; Title = english ? "Settings" : "设置"; TitleText.Text = english ? "Settings" : "设置";
        SearchBox.ToolTip = english ? "Search settings" : "搜索设置"; SearchBox.Text = "";
        LanguageLabel.Text = english ? "Language" : "语言"; ThemeLabel.Text = english ? "Theme" : "主题"; ColorLabel.Text = english ? "Chat color" : "聊天颜色"; EncryptionLabel.Text = english ? "Message encryption" : "消息加密";
        LanguageButton.Content = english ? "中文" : "English"; ThemeButton.Content = _settings.DarkMode ? (english ? "Light" : "白色") : (english ? "Dark" : "黑色"); ColorButton.Content = english ? "Apply" : "应用"; EncryptionBox.Content = english ? "Enabled" : "已启用"; CloseButton.Content = english ? "Close" : "关闭";
    }

    private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        var query = SearchBox.Text.Trim(); SetVisible(LanguageRow, "language 语言 中文 english", query); SetVisible(ThemeRow, "theme 主题 dark light 黑色 白色", query); SetVisible(ColorRow, "color 颜色 chat 聊天", query); SetVisible(EncryptionRow, "encryption 加密 message 消息", query);
    }

    private static void SetVisible(UIElement element, string keywords, string query) => element.Visibility = string.IsNullOrWhiteSpace(query) || keywords.Contains(query, StringComparison.OrdinalIgnoreCase) ? Visibility.Visible : Visibility.Collapsed;

    private void LanguageButton_Click(object sender, RoutedEventArgs e) { _localization.SetLanguage(!_localization.IsEnglish); ApplyLanguage(); }
    private void ThemeButton_Click(object sender, RoutedEventArgs e) { _settings.DarkMode = !_settings.DarkMode; _applyTheme(_settings.DarkMode); ApplyLanguage(); }
    private void ColorButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (ColorConverter.ConvertFromString(ColorBox.Text.Trim()) is not Color color) throw new FormatException();
            _settings.ChatColor = ColorBox.Text.Trim(); _applyTheme(_settings.DarkMode); Owner.Resources["AccentBrush"] = new SolidColorBrush(color); StatusText.Text = _localization.IsEnglish ? "Chat color applied." : "聊天颜色已应用。";
        }
        catch (FormatException) { StatusText.Text = _localization.IsEnglish ? "Invalid color. Use values such as #2563EB or SteelBlue." : "颜色格式无效，请输入 #2563EB 或 SteelBlue。"; }
        catch (Exception) { StatusText.Text = _localization.IsEnglish ? "The color could not be applied." : "颜色应用失败。"; }
    }
    private void CloseButton_Click(object sender, RoutedEventArgs e) { _settings.EncryptMessages = EncryptionBox.IsChecked == true; DialogResult = true; }
}
