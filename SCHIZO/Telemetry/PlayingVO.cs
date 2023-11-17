namespace SCHIZO.Telemetry;

partial class PlayingVO
{
    // SN ency logs don't use subtitles
    private SoundQueue Sounds => PDASounds.queue;
    // some specific BZ cutscenes don't use SoundQueue
    // now... technically this won't work if subtitles are off; however: just don't disable subtitles
    private uGUI_MessageQueue Subs => Subtitles.main ? Subtitles.main.queue : null;
    public bool IsPlaying => IsPlayingVO || IsShowingSubtitles;
    private bool IsShowingSubtitles => Subs is { messages.Count: > 0 };
    private bool IsPlayingVO
#if BELOWZERO
            => Sounds is { _current.host: SoundHost.Encyclopedia or SoundHost.Log or SoundHost.Realtime };
#else
            => Sounds is { _current: { }, _lengthSeconds: > 1f };
#endif

    private bool _wasPlaying;

    private void FixedUpdate()
    {
        bool isPlaying = IsPlaying;
        if (isPlaying == _wasPlaying) return;

        Send(isPlaying);
        _wasPlaying = isPlaying;
    }

    protected override void OnDisable()
    {
        if (_wasPlaying) Send(false);
        _wasPlaying = false;
    }

    private void OnDestroy() => OnDisable();

    // private float TimeLeft(uGUI_MessageQueue.Message msg) => msg is null ? 0 : msg.duration + msg.delay - msg.time;
    // public float TimeLeftNext() => Subtitles?.messages.Select(TimeLeft).FirstOrFallback(0) ?? 0;
    // public float TimeLeftAll() => Subtitles?.messages.Sum(TimeLeft) ?? 0;

    private void Send(bool isPlaying)
    {
        SendTelemetry("playingVO", new { playing = isPlaying });
    }
}
