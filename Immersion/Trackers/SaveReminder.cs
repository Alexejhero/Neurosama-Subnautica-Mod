using Immersion.Formatting;
using Nautilus.Utility;

namespace Immersion.Trackers;

public sealed class SaveReminder : Tracker
{
    private float _lastSave;
    private bool _inGame;
    private int _currentReminderIndex = -1;
    private readonly float[] _defaultReminders = [1800, 3600]; // 30m, 1h
    private float[] _currentReminders; // slightly randomized for variety

    private void Start()
    {
        SaveUtils.RegisterOnFinishLoadingEvent(Loaded);
        SaveUtils.RegisterOnSaveEvent(Saved);
        SaveUtils.RegisterOnQuitEvent(Quit);
        _currentReminders = new float[_defaultReminders.Length];
        //_defaultReminders.CopyTo(_currentReminders, 0);
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
        Reroll();
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
    }

    private void Reroll()
    {
        for (int i = 0; i < _currentReminders.Length; i++)
        {
            float t = _defaultReminders[i];
            _currentReminders[i] = Random.Range(t * 0.9f, t * 1.1f);
        }
        //LOGGER.LogDebug($"Rerolled save reminder ({string.Join(",", _currentReminders)})");
    }
}
