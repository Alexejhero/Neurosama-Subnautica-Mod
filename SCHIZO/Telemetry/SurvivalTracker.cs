using UnityEngine;

namespace SCHIZO.Telemetry;

partial class SurvivalTracker
{
    private float _timeLastNotify;
    private Survival survival;
    private LiveMixin liveMixin;

    private State _lastHealth;
    private State _lastFood;
    private State _lastWater;

    private void Awake()
    {
        _timeLastNotify = -float.MaxValue;
    }

    public void FixedUpdate()
    {
        if (Time.fixedTime < _timeLastNotify) return;
        if (!survival && !liveMixin && !Init()) return;

        UpdateHealth();
        UpdateSurvival();
    }

    private void UpdateHealth()
    {
        if (!liveMixin) return;

        if (!liveMixin.IsAlive())
        {
            if (_lastHealth == State.Depleted) return;
            _lastHealth = State.Depleted;
            NotifyDeath();
            return;
        }

        _lastHealth = UpdateProperty("health", liveMixin.health / liveMixin.maxHealth, _lastHealth, healthThresholds);
    }

    private void UpdateSurvival()
    {
        if (!survival) return;

        _lastFood = UpdateProperty("food", survival.food / 100f, _lastFood, foodThresholds);
        _lastWater = UpdateProperty("water", survival.water / 100f, _lastWater, waterThresholds);
    }

    private bool Init()
    {
        Player target = Player.main;
        if (!target) return false;
        survival = target.GetComponent<Survival>();
        liveMixin = target.GetComponent<LiveMixin>();
        return true;
    }

    private State UpdateProperty(string name, float value, State last, Vector2 thresholds)
    {
        State state = Check(value, thresholds);
        if (state != last)
            Notify(name, value, state);
        return state;
    }

    private State Check(float value, Vector2 thresholds)
    {
        (float critical, float low) = (thresholds.x, thresholds.y);
        return value < critical ? State.Critical
            : value < low ? State.Low
            : State.Normal;
    }

    private void Notify(string property, float value, State state)
    {
        if (state == State.Normal) return;

        float interval = state == State.Critical ? notifyCriticalInterval : notifyLowInterval;
        if (Time.fixedTime - _timeLastNotify > interval)
            Send($"{property}{state}", value);
    }

    private void NotifyDeath()
    {
        Send($"died");
    }

    private enum State
    {
        Normal,
        Low,
        Critical,
        Depleted
    }
}
