using System.Collections;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace SCHIZO.Telemetry;

partial class TelemetryCoordinator
{
    private void Awake()
    {
        if (!baseUrl.EndsWith("/")) baseUrl += "/";
    }

    public IEnumerator Send(string path, object data = null, UnityWebRequest.UnityWebRequestMethod method = UnityWebRequest.UnityWebRequestMethod.Post)
    {
        if (!enabled) yield break;

        string fullUri = baseUrl + path;
        string dataJson = data == null ? "" : JsonConvert.SerializeObject(data);

        LOGGER.LogWarning($"POST {fullUri}\n{dataJson}");
        UnityWebRequest request = UnityWebRequest.Post(fullUri, dataJson);

        yield return request.SendWebRequest();
    }
}
