using Immersion.Formatting;
using Nautilus.Utility;

namespace Immersion.Trackers;

public sealed class SaveReminder : Tracker
{
    private float _lastSave;
    private bool _inGame;
    private int _currentReminderIndex = -1;
    private readonly float[] _defaultReminders = [1200, 2400, 3600]; // 20m, 40m, 1h
    private float[] _currentReminders; // slightly randomized for variety

    private void Start()
    {
        SaveUtils.RegisterOnFinishLoadingEvent(Loaded);
        SaveUtils.RegisterOnSaveEvent(Saved);
        SaveUtils.RegisterOnQuitEvent(Quit);
        Reroll();
    }

    private void OnDestroy()
    {
        SaveUtils.UnregisterOnFinishLoadingEvent(Loaded);
        SaveUtils.UnregisterOnSaveEvent(Saved);
        SaveUtils.UnregisterOnQuitEvent(Quit);
    }

    private void Saved()
    {
        _lastSave = PDA.time;
        _currentReminderIndex = -1;
    }

    private void Loaded()
    {
        Saved();
        _inGame = true;
    }

    private void Quit()
    {
        Saved();
        _inGame = false;
    }

    private void FixedUpdate()
    {
        if (!_inGame) return;

        float timeSinceLastSave = PDA.time - _lastSave;
        for (int i = _currentReminders.Length - 1; i > _currentReminderIndex; i--)
        {
            if (timeSinceLastSave < _currentReminders[i]) continue;

            _currentReminderIndex = i;
            SendReminder(timeSinceLastSave);
            return;
        }
    }

    internal void SendReminder(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        string message = $"{{player}} has not saved in over {timeSpan.ToFriendlyString(1)}.";
        React(Priority.Low, Format.FormatPlayer(message));
        _currentReminders = Reroll();
    }

    private float[] Reroll()
    {
        return _defaultReminders
            .Select(t => Random.Range(t * 0.9f, t * 1.1f))
            .ToArray();
    }
}
