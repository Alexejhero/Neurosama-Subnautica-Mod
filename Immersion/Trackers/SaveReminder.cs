using Immersion.Formatting;
using Nautilus.Utility;

namespace Immersion.Trackers;

public sealed class SaveReminder : Tracker
{
    private float _lastSave;
    private bool _inGame;
    private int _currentReminderIndex = -1;
    private readonly float[] _reminders = [1200, 2400, 3600]; // 20m, 40m, 1h

    private void Start()
    {
        SaveUtils.RegisterOnFinishLoadingEvent(Loaded);
        SaveUtils.RegisterOnSaveEvent(Saved);
        SaveUtils.RegisterOnQuitEvent(Quit);
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
        for (int i = _reminders.Length - 1; i > _currentReminderIndex; i--)
        {
            if (timeSinceLastSave < _reminders[i]) continue;

            _currentReminderIndex = i;
            SendReminder(timeSinceLastSave);
            return;
        }
    }

    internal void SendReminder(float time)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        string message = $"{{player}} has not saved in over {timeSpan}.";
        React(Priority.Low, Format.FormatPlayer(message));
    }
}
