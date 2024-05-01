using Immersion.Formatting;

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
        { }
        public string Name { get; set; } = name;
        public (float Critical, float Low) Thresholds { get; set; } = (critical, low);
        public float Value => valueGetter();
        public float MaxValue => maxValueGetter();
        public float Percentage => Value / MaxValue;

        internal BarState lastState;
    }

    private LiveMixin liveMixin;
    private Survival survival;
    private BodyTemperature bodyTemp;

    private List<Bar> bars;

    private float _nextUpdate;
    private float _updateInterval = 1f;

    protected override void Awake()
    {
        base.Awake();

        bars = [
            new Bar("health",
                valueGetter: () => liveMixin.health,
                maxValueGetter: () => liveMixin.maxHealth,
                critical: 0.15f, low: 0.30f),
            new Bar("food",
                valueGetter: () => survival.food,
                maxValue: 100,
                critical: 0.10f, low: 0.30f),
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
        if (Player.main.liveMixin.IsAlive()) return false;

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
        (float critical, float low) = bar.Thresholds;
        BarState state = value < critical ? BarState.Critical
            : value < low ? BarState.Low
            : BarState.Normal;

        if (state > bar.lastState)
            Notify(bar.Name, state);
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
