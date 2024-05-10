using System.Collections;
using Immersion.Formatting;
using Immersion.Patches;
using UWE;

namespace Immersion.Trackers;

public sealed class Backseating : Tracker
{
    private enum BarState
    {
        Normal,
        Low,
        Critical,
        Depleted // used for health to signal death
    }

    private class Bar(string name, Func<float> valueGetter, Func<float> maxValueGetter, float critical, float low)
    {
        public Bar(string name, Func<float> valueGetter, float maxValue, float critical, float low)
            : this(name, valueGetter, () => maxValue, critical, low)
        {
            RerollThresholds();
        }
        public string Name { get; set; } = name;
        public (float Critical, float Low) Thresholds { get; set; } = (critical, low);
        public (float Critical, float Low) CurrentThresholds { get; set; } = (critical, low);
        public float Value => valueGetter();
        public float MaxValue => maxValueGetter();
        public float Percentage => Value / MaxValue;

        internal BarState lastState;

        public void RerollThresholds()
        {
            (float crit, float low) = Thresholds;
            (float currCrit, float currLow) = CurrentThresholds;
            float critRoll = Random.Range(crit * 0.8f, currCrit);
            float lowRoll = Random.Range(low * 0.8f, currLow);
            CurrentThresholds = (critRoll, lowRoll);
            LOGGER.LogDebug($"{Name} rolled ({CurrentThresholds.Critical}, {CurrentThresholds.Low})");
        }
    }

    public static Backseating Instance { get; private set; }

    private LiveMixin liveMixin;
    private Survival survival;
    private BodyTemperature bodyTemp;

    private List<Bar> bars;

    private float _nextUpdate;
    private float _updateInterval = 1f;

    protected override void Awake()
    {
        base.Awake();
        Instance = this;

        bars = [
            new Bar("health",
                valueGetter: () => liveMixin.health,
                maxValueGetter: () => liveMixin.maxHealth,
                critical: 0.20f, low: 0.40f),
            new Bar("food",
                valueGetter: () => survival.food,
                maxValue: 100,
                critical: 0.10f, low: 0.20f), // drains slower than water
            new Bar("water",
                valueGetter: () => survival.water,
                maxValue: 100,
                critical: 0.10f, low: 0.30f),
            new Bar("body temperature",
                valueGetter: () => bodyTemp.currentBodyHeatValue,
                maxValueGetter: () => bodyTemp.maxBodyHeatValue,
                critical: 0.10f, low: 0.25f),
            // it's also possible to track things like the currently equipped tool's energy charge
        ];
    }

    private void OnEnable()
    {
        UpdateBaseGameAlerts();
    }

    private void OnDisable()
    {
        UpdateBaseGameAlerts();
    }

    private void UpdateBaseGameAlerts()
    {
        ToggleSurvivalAlerts(!enabled);
        CoroutineHost.StartCoroutine(ToggleHypothermiaAlerts(!enabled));
    }

    private void ToggleSurvivalAlerts(bool enabled)
    {
        SurvivalAlertPatches.EnableSurvivalAlerts = enabled;
    }
    private IEnumerator ToggleHypothermiaAlerts(bool enabled)
    {
        yield return new WaitUntil(() => bodyTemp);
        PDANotification notif = bodyTemp.hypothermiaWarningNotification;
        notif.nextPlayTime = enabled ? 0 : float.PositiveInfinity;
    }

    public void FixedUpdate()
    {
        float time = Time.fixedTime;
        if (time < _nextUpdate) return;

        _nextUpdate = time + _updateInterval;

        if (!Player.main) return;
        if (!liveMixin) liveMixin = Player.main.liveMixin;
        if (!survival) survival = Player.main.GetComponent<Survival>();
        if (!bodyTemp) bodyTemp = Player.main.GetComponent<BodyTemperature>();

        foreach (Bar bar in bars)
        {
            if (bar.Name == "health" && UpdateDeath(bar)) continue;

            UpdateBar(bar);
        }
    }

    private bool UpdateDeath(Bar healthBar)
    {
        if (!liveMixin || liveMixin.IsAlive()) return false;

        if (healthBar.lastState != BarState.Depleted)
        {
            healthBar.lastState = BarState.Depleted;
            NotifyDeath();
        }
        return true;
    }

    private void UpdateBar(Bar bar)
    {
        float value = bar.Percentage;
        (float critical, float low) = bar.CurrentThresholds;
        BarState state = value < critical ? BarState.Critical
            : value < low ? BarState.Low
            : BarState.Normal;

        if (state > bar.lastState)
        {
            Notify(bar.Name, state);
        }
        else if (state < bar.lastState)
        {
            // bar went up, we can reroll the threshold to be lower
            bar.RerollThresholds();
        }
        bar.lastState = state;
    }

    private void Notify(string property, BarState state)
    {
        string message = Format.FormatPlayer($"{{player}}'s {property} is {FormatState(state)}.");
        React(state > BarState.Low ? Priority.High : Priority.Low, message);
    }

    private void NotifyDeath()
    {
        React(Priority.High, Format.FormatPlayer("{player} has died."));
    }

    private string FormatState(BarState state)
    {
        return state switch
        {
            BarState.Normal => "OK",
            BarState.Low => "low",
            BarState.Critical => "critically low",
            BarState.Depleted => "rock-bottom",
            _ => "really weird and off-putting",
        };
    }
}
