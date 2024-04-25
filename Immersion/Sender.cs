using System.Collections;
using System.Net.Http;
using UnityEngine.Networking;

namespace Immersion;

internal class Sender : MonoBehaviour
{
    private static HttpClient _client;

    /// <summary>
    /// Force the next <see cref="Send"/> call to proceed even if the component is disabled.
    /// </summary>
    public bool forceNext;
    private Uri _baseUri;

    public void Awake()
    {
        _baseUri = new(Globals.BaseUrl);
        _client = new HttpClient { BaseAddress = new Uri(Globals.BaseUrl) };
        _client.DefaultRequestHeaders.UserAgent.Add(new("Unity", Application.unityVersion));
        _client.DefaultRequestHeaders.UserAgent.Add(new("(SCHIZO.Immersion)"));
    }

    public void Send(string path, object data = null)
    {
        if (!enabled && !forceNext) return;
        forceNext = false;

        Uri uri = new(_baseUri, path);
        string postData = data?.ToString() ?? "";
        StartCoroutine(SendCoro(uri, postData));
    }

    private IEnumerator SendCoro(Uri uri, string data)
    {
        using UnityWebRequest req = UnityWebRequest.Post(uri, data);
        req.SetRequestHeader("User-Agent", $"Unity/{Application.unityVersion} (SCHIZO.Immersion)");
        LOGGER.LogWarning($"POST {uri}\n{data}");
        yield return req.SendWebRequest();
    }
}
