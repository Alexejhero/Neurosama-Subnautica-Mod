using System.Collections;
using Immersion.Formatting;

namespace Immersion.Trackers;

/// <summary>
/// Replace the game's oxygen alerts with calls to the LLM.<br/>
/// Replaces both <see cref="LowOxygenAlert"/> (PDA alerts) and <see cref="HintSwimToSurface"/> (UI hint).<br/>
/// Restores the base game alerts when disabled.
/// </summary>
public sealed class OxygenAlerts : Tracker
{
    private List<Alert> _ourAlerts;
    private List<LowOxygenAlert.Alert> _gameAlerts;

    private LowOxygenAlert _alerts;
    private Player _player;

    public static OxygenAlerts Instance { get; private set; }

    protected override void Awake()
    {
        Instance = this;
        base.Awake();
        _ourAlerts = [
            new Alert {
                Message = "{player}'s oxygen tank is under half capacity.",
                Priority = Priority.Low,
                minDepth = 50, // base game's 30s alert has a min depth of 30
                minO2Capacity = 60,
                notification = null,
                oxygenTriggerSeconds = 0, // originally 30s, changed to trigger from tank fill % instead
                OxygenTriggerProportion = 0.5f,
                MuteIfOxygenSourcesNearby = true,
            },
            new Alert {
                Message = "{player}'s oxygen is about to run out.",
                // minDepth = 15, // base game's is 0
                minO2Capacity = 30,
                notification = null, // TODO: play the alarm sound but not the PDA voiceline (create PDANotification that plays only one of the two sounds)
                oxygenTriggerSeconds = 10,
                MuteIfOxygenSourcesNearby = false,
                Cooldown = 30,
            }
        ];
        _ourAlerts[0].OnPlay = RerollOxygenProportion;
    }

    private static void RerollOxygenProportion(Alert alert)
    {
        // decay by 5% of current value each reroll down to a minimum of 0.25 fill (25% of capacity)
        alert.OxygenTriggerProportion = Random.Range(Mathf.Max(0.25f, alert.OxygenTriggerProportion * 0.95f), alert.OxygenTriggerProportion);
        LOGGER.LogDebug($"New oxy threshold ({alert.OxygenTriggerProportion*100:f2}%)");
    }

    private void OnEnable()
    {
        StartCoroutine(InitCoro());
    }

    private IEnumerator InitCoro()
    {
        yield return new WaitUntil(() => Player.main);
        _player = Player.main;
        if (!_alerts)
            _alerts = Player.main.GetComponentInChildren<LowOxygenAlert>();
        _gameAlerts = _alerts.alertList;
        _alerts.alertList = [];
    }

    private void OnDisable()
    {
        if (!_alerts) return; // cleaning up, we don't need to do anything

        _alerts.alertList = _gameAlerts;
    }

    private readonly Utils.ScalarMonitor _secondsMonitor = new(100f);
    private readonly Utils.ScalarMonitor _proportionMonitor = new(1f);
    private int _lastAlert = -1;

    private void Update()
    {
        if (!_player || !_player.IsAlive()) return;
        if (_player.CanBreathe()) return;
        if (!GameModeManager.GetOption<bool>(GameOption.OxygenDepletes)) return;

        float time = Time.time;

        // prevent 30s alert from triggering right after the 10s one
        float lastAlertCooldownTimer = _lastAlert >= 0 && _lastAlert < _ourAlerts.Count
            ? _ourAlerts[_lastAlert].CooldownTimer
            : 0;
        bool lastAlertOnCooldown = lastAlertCooldownTimer > time;

        float capacity = _player.GetOxygenCapacity();
        float has = _player.GetOxygenAvailable();
        float drain = _player.GetOxygenPerBreath(_player.GetBreathPeriod(), _player.depthClass.value);
        float proportion = has / capacity;
        _proportionMonitor.Update(proportion);

        float emptyInSeconds = Mathf.Max(0, has / drain);
        _secondsMonitor.Update(emptyInSeconds);
        for (int i = _ourAlerts.Count - 1; i >= 0; i--)
        {
            Alert alert = _ourAlerts[i];
            if (alert.CooldownTimer > time) continue;
            if (_lastAlert > i && lastAlertOnCooldown) continue;

            if (alert.minO2Capacity > capacity) continue;
            bool isProportionTrigger = alert.oxygenTriggerSeconds <= 0;
            if (isProportionTrigger)
            {
                if (alert.OxygenTriggerProportion < proportion) continue;
                // if (!_proportionMonitor.JustDroppedBelow(alert.OxygenTriggerProportion)) break;
            }
            else
            {
                if (alert.oxygenTriggerSeconds < emptyInSeconds) continue;
                // if (!_secondsMonitor.JustDroppedBelow(alert.oxygenTriggerSeconds)) break;
            }

            float depth = _player.GetDepth();
            if (alert.minDepth > depth) continue;

            if (alert.MuteIfOxygenSourcesNearby)
            {
                if (alert.NextCheck > time) continue;
                alert.NextCheck = time + 2f;

                float swimDistance = emptyInSeconds * 5; // 5 m/s is very generous, base swim speed is 6.64 and seaglide is 25

                if (swimDistance > depth)
                {
                    //LOGGER.LogDebug($"Close to surface {swimDistance} > {depth}");
                    continue;
                }

                // whoever is reading this in probably like 2040 or something - do you think anyone ever noticed this doesn't actually get the nearest one
                IInteriorSpace interior = _player.GetRespawnInterior();
                bool interiorIsBreathable = interior switch
                {
                    SubRoot sub => sub.powerRelay && sub.powerRelay.IsPowered(),
                    Vehicle vehicle => vehicle.IsPowered(),
                    SeaTruckSegment truck => truck.relay && truck.relay.IsPowered(),
                    _ => false,
                };
                if (interiorIsBreathable)
                {
                    float dist = (interior.GetGameObject().transform.position - _player.transform.position).magnitude;
                    if (swimDistance > dist)
                    {
                        //LOGGER.LogDebug($"Base close by {swimDistance} > {dist}");
                        continue;
                    }
                }
            }

            _lastAlert = i;
            Play(alert);
            break;
        }
    }

    internal void Play(Alert alert)
    {
        if (alert.notification) alert.notification.Play();
        alert.CooldownTimer = Time.time + alert.Cooldown;
        alert.OnPlay?.Invoke(alert);

        React(alert.Priority, Format.FormatPlayer(alert.Message));
    }
    internal sealed class Alert : LowOxygenAlert.Alert
    {
        public required string Message { get; set; }
        public Priority Priority { get; set; } = Priority.High;
        public float OxygenTriggerProportion { get; set; } = 1;
        /// <summary>
        /// Includes the surface and nearby breathable interiors (base, vehicle, etc).
        /// </summary>
        public bool MuteIfOxygenSourcesNearby { get; set; }
        public float NextCheck { get; set; }
        public float Cooldown { get; set; } = 60;
        public float CooldownTimer { get; set; }

        public Action<Alert> OnPlay { get; set; }
    }
}
