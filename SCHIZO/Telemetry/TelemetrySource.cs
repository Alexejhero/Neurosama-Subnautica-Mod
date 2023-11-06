namespace SCHIZO.Telemetry;

partial class TelemetrySource
{
    protected TelemetryCoordinator _coordinator;

    private void Awake()
    {
        _coordinator = GetComponent<TelemetryCoordinator>();
    }

    public void SendTelemetry(string path, object data = null)
    {
        if (!enabled) return;

        StartCoroutine(_coordinator.CoSendTelemetry(path, data));
    }
}
