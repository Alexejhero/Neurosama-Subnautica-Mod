using UnityEngine.Networking;

namespace SCHIZO.Telemetry;

partial class TelemetrySource
{
    public void Send(string path, object data = null, UnityWebRequest.UnityWebRequestMethod method = UnityWebRequest.UnityWebRequestMethod.Post)
        => StartCoroutine(sender.Send($"{category}/{path}", data, method));
}
