using System.Net.Http;
using Newtonsoft.Json;

namespace Immersion;

internal class Sender : MonoBehaviour
{
    public const string BASE_URL = "http://localhost:8000/subnautica/";

    private static object _empty = new();
    private static HttpClient _client;

    public void Awake()
    {
        _client = new HttpClient { BaseAddress = new Uri(BASE_URL) };
    }

    public async Task Send(string path, object data = null)
    {
        if (!enabled) return;

        string fullUri = BASE_URL + path;
        string dataJson = JsonConvert.SerializeObject(data ?? _empty);
#warning DEV
        LOGGER.LogWarning($"POST {fullUri}\n{dataJson}");

        await _client.PostAsync(path, new StringContent(dataJson));
    }
}
