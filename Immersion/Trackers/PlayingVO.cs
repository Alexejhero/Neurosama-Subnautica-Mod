namespace Immersion.Trackers;

public sealed class PlayingVO : Tracker
{
    internal const string ENDPOINT = "playingVO";
    // SN ency logs don't use subtitles
    private SoundQueue Sounds => PDASounds.queue;
    // some specific BZ cutscenes don't use SoundQueue
    // now... technically this won't work if subtitles are off; however: just don't disable subtitles
    private uGUI_MessageQueue Subs => Subtitles.main ? Subtitles.main.queue : null;
    public bool IsPlaying => IsPlayingVO || IsShowingSubtitles;
    public bool IsShowingSubtitles => Subs is { messages.Count: > 0 };
    public bool IsPlayingVO => Sounds is { _current.host: SoundHost.Encyclopedia or SoundHost.Log or SoundHost.Realtime };

    private bool _wasPlaying;

    private void FixedUpdate()
    {
        bool isPlaying = IsPlaying;
        if (isPlaying == _wasPlaying) return;

        Send(isPlaying);
        _wasPlaying = isPlaying;
    }

    private void OnDisable()
    {
        if (_wasPlaying) Send(false);
        _wasPlaying = false;
    }

    internal void Send(bool isPlaying)
    {
        Send(ENDPOINT, new { playing = isPlaying });
    }
}
