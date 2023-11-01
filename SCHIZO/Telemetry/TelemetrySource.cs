namespace SCHIZO.Telemetry;

partial class TelemetrySource
{
    public void Send(string path, object data = null)
    {
        if (!enabled) return;

        StartCoroutine(coordinator.Send(path, data));
    }
}
