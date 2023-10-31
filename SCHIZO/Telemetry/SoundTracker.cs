using System;
using System.Linq;

namespace SCHIZO.Telemetry;

partial class SoundTracker
{
    private SoundQueue Sounds => PDASounds.queue;
    // some specific BZ cutscenes don't use SoundQueue
    private uGUI_MessageQueue Subtitles => global::Subtitles.main ? global::Subtitles.main.queue : null;
    public bool IsPlaying => IsPlayingSounds() || IsShowingSubtitles;
    public bool IsShowingSubtitles => Subtitles is { messages.Count: > 0 };
    private partial bool IsPlayingSounds();
    private bool _wasPlaying;

    public Action onPlay;
    public Action onStop;

    private void Awake()
    {
        
    }

    private void FixedUpdate()
    {
        bool isPlaying = IsPlaying;
        if (isPlaying == _wasPlaying) return;

        (isPlaying ? onPlay : onStop)?.Invoke();
        Send(isPlaying ? "playing" : "stopped");
        _wasPlaying = isPlaying;
    }

    private void OnDisable()
    {
        if (_wasPlaying) Send("stopped");
    }

    private float TimeLeft(uGUI_MessageQueue.Message msg) => msg is null ? 0 : msg.duration + msg.delay - msg.time;
    public float TimeLeftNext() => Subtitles?.messages.Select(TimeLeft).FirstOrFallback(0) ?? 0;
    public float TimeLeftAll() => Subtitles?.messages.Sum(TimeLeft) ?? 0;
}
