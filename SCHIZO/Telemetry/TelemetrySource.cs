namespace SCHIZO.Telemetry;

partial class TelemetrySource
{
    public void Send(string path, object data = null)
    {
        if (!enabled) return;
        string fullPath = string.IsNullOrEmpty(category) ? path : $"{category}/{path}";
        StartCoroutine(coordinator.Send(fullPath, data));
    }
}
