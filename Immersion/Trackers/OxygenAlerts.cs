using System.Collections;
using Immersion.Formatting;

namespace Immersion.Trackers;

public sealed class OxygenAlerts : Tracker
{
    private List<LowOxygenAlert.Alert> _ourAlerts;
    private List<LowOxygenAlert.Alert> _gameAlerts;

    private LowOxygenAlert _alerts;

    protected override void Awake()
    {
        base.Awake();
        _ourAlerts = [
            new Alert(this, "{player} has 30 seconds of oxygen remaining.") {
                minDepth = 50, // base game's 30s alert has a min depth of 30
                minO2Capacity = 60,
                notification = null,
                oxygenTriggerSeconds = 30,
            },
            new Alert(this, "{player}'s oxygen is about to run out.") {
                minDepth = 15, // base game's is 0
                minO2Capacity = 30,
                notification = null, // TODO: play the alarm sound but not the PDA voiceline
                oxygenTriggerSeconds = 10,
            }
        ];
    }

    private void OnEnable()
    {
        StartCoroutine(InitCoro());
    }

    private IEnumerator InitCoro()
    {
        yield return new WaitUntil(() => Player.main);
        _alerts = Player.main.GetComponentInChildren<LowOxygenAlert>();
        _gameAlerts = _alerts.alertList;
        _alerts.alertList = _ourAlerts;
    }

    private void OnDisable()
    {
        if (!_alerts) return; // cleaning up, we don't need to do anything

        _alerts.alertList = _gameAlerts;
    }

    private void Notify(Alert alert)
    {
        React(alert.Priority, Format.FormatPlayer(alert.Message));
    }
    internal sealed class Alert(OxygenAlerts owner, string message, Priority priority = Priority.Low) : LowOxygenAlert.Alert
    {
        public string Message { get; } = message;
        public Priority Priority { get; } = priority;
        public void Play() => owner.Notify(this);
    }
}
