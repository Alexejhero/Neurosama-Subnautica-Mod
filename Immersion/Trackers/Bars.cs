namespace Immersion.Trackers;

public sealed class Bars : Tracker
{
    private const string ENDPOINT = "react";

    private enum BarState
    {
        Normal,
        Low,
        Critical,
        Depleted // used for health to signal death
    }

    private class Bar(string name, Func<float> valueGetter, Func<float> maxValueGetter, float critical, float low)
    {
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
            new Bar("health", () => liveMixin.health, () => liveMixin.maxHealth, 0.25f, 0.5f),
            new Bar("food", () => survival.food, () => 100, 0.1f, 0.3f),
            new Bar("water", () => survival.water, () => 100, 0.2f, 0.4f),
            new Bar("body temperature", () => bodyTemp.currentBodyHeatValue, () => bodyTemp.maxBodyHeatValue, 0.1f, 0.25f),
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
        Send(ENDPOINT, $"{Globals.PlayerName}'s {property} is {FormatState(state)}.");
    }

    private void NotifyDeath()
    {
        Send(ENDPOINT, $"{Globals.PlayerName} has died.");
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
