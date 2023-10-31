using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace SCHIZO.Telemetry;

partial class TelemetrySender
{
    public IEnumerator Send(string path, object data = null, UnityWebRequest.UnityWebRequestMethod method = UnityWebRequest.UnityWebRequestMethod.Post)
    {
        string fullUri = baseUrl + path;
        string dataJson = JsonConvert.SerializeObject(data);

        //LOGGER.LogWarning($"{method.ToString().ToUpperInvariant()} {fullUri}\n{dataJson}");
        UnityWebRequest request = method switch
        {
            UnityWebRequest.UnityWebRequestMethod.Get => UnityWebRequest.Get(fullUri),
            UnityWebRequest.UnityWebRequestMethod.Post => UnityWebRequest.Post(fullUri, dataJson),
            _ => throw new NotSupportedException($"Method {method} is not supported")
        };

        yield return request.SendWebRequest();
    }
}
