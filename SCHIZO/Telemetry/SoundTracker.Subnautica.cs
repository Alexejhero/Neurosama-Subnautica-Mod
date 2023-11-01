namespace SCHIZO.Telemetry;

partial class SoundTracker
{
    private partial bool IsPlayingSounds()
    {
        return Sounds is { _current: { }, _lengthSeconds: > 0.5f };
    }
}
