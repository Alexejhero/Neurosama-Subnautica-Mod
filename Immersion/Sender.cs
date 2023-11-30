using System.Net.Http;
using Newtonsoft.Json;

namespace Immersion;

internal class Sender : MonoBehaviour
{
    private static object _empty = new();
    private static HttpClient _client;

    /// <summary>
    /// Force the next <see cref="Send"/> call to proceed even if the component is disabled.
    /// </summary>
    public bool forceNext;

    public void Awake()
    {
        _client = new HttpClient { BaseAddress = new Uri(Globals.BaseUrl) };
        _client.DefaultRequestHeaders.UserAgent.Add(new ("Unity", Application.unityVersion));
    }

    public async Task Send(string path, object data = null)
    {
        if (!enabled && !forceNext) return;
        forceNext = false;

        string dataJson = JsonConvert.SerializeObject(data ?? _empty);

        LOGGER.LogWarning($"POST {_client.BaseAddress + path}\n{dataJson}");
        await _client.PostAsync(path, new StringContent(dataJson));
    }
}
