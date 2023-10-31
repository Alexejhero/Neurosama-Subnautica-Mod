namespace SCHIZO.Telemetry;
partial class SoundTracker
{
    private partial bool IsPlayingSounds()
    {
        return Sounds is { _current: string, _lengthSeconds: > 0.5f };
    }
}
