using System.Net.Http;

namespace Immersion;

internal class Sender : MonoBehaviour
{
    private static HttpClient _client;

    /// <summary>
    /// Force the next <see cref="Send"/> call to proceed even if the component is disabled.
    /// </summary>
    public bool forceNext;

    public void Awake()
    {
        _client = new HttpClient { BaseAddress = new Uri(Globals.BaseUrl) };
        _client.DefaultRequestHeaders.UserAgent.Add(new("Unity", Application.unityVersion));
        _client.DefaultRequestHeaders.UserAgent.Add(new("(SCHIZO.Immersion)"));
    }

    public async Task Send(string path, object data = null)
    {
        if (!enabled && !forceNext) return;
        forceNext = false;

        string dataString = data?.ToString() ?? "";
        LOGGER.LogWarning($"POST {new Uri(_client.BaseAddress, path)}\n{dataString}");
        await _client.PostAsync(path, new StringContent(dataString));
    }
}
