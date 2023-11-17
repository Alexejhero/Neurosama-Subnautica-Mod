namespace SCHIZO.Telemetry;

partial class TelemetrySource<T>
    where T : TelemetrySource<T>
{
    protected static T instance;
    protected TelemetryCoordinator _coordinator;

    protected virtual void Awake()
    {
        instance = (T)this;
        _coordinator = GetComponent<TelemetryCoordinator>();
    }

    public void SendTelemetry(string path, object data = null)
    {
        if (!enabled) return;

        StartCoroutine(_coordinator.CoSendTelemetry(path, data));
    }
}
