using System.Collections;
using UnityEngine.Networking;

namespace Immersion;

internal class Sender : MonoBehaviour
{
    /// <summary>
    /// Force the next <see cref="Send"/> call to proceed even if the component is disabled.
    /// </summary>
    public bool forceNext;

    public void Send(string path, object data = null)
    {
        if (!enabled && !forceNext) return;
        forceNext = false;

        Uri uri = new(new(Globals.BaseUrl), path);
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
