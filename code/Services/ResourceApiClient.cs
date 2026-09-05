using System.Net.Http;

namespace ChatClient.Services;

public sealed class ResourceApiClient
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(15) };
    private static readonly Uri BaseUri = new("https://shit.pub/s/developer/Client/ChatClient/API/s/");
    private DateTime _lastRequest = DateTime.MinValue;

    public async Task<string> GetAsync(string resource, CancellationToken cancellationToken = default)
    {
        resource = resource.Trim().Trim('/');
        if (string.IsNullOrWhiteSpace(resource)) return "请输入要兑换的资源名称。";
        if (resource.Any(c => !(char.IsLetterOrDigit(c) || c is '-' or '_' or '.')))
            return "资源名称包含不支持的字符。";
        var remaining = TimeSpan.FromMinutes(1) - (DateTime.UtcNow - _lastRequest);
        if (remaining > TimeSpan.Zero) return $"兑换操作每分钟一次，请 {Math.Ceiling(remaining.TotalSeconds)} 秒后重试。";
        _lastRequest = DateTime.UtcNow;
        try
        {
            using var response = await Http.GetAsync(new Uri(BaseUri, $"{Uri.EscapeDataString(resource)}/txt.txt"), cancellationToken);
            if (!response.IsSuccessStatusCode) return "抱歉，您搜索的内容貌似没找到。";
            var text = await response.Content.ReadAsStringAsync(cancellationToken);
            return string.IsNullOrWhiteSpace(text) ? "抱歉，您搜索的内容貌似没找到。" : text;
        }
        catch (HttpRequestException) { return "资源服务暂时不可用，请稍后重试。"; }
        catch (TaskCanceledException) { return "请求超时，请检查网络后重试。"; }
    }
}
