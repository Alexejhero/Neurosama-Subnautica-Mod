using System;
using System.Linq;
using System.Reflection;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Telemetry;

partial class Bars
{
    private enum BarState
    {
        Normal,
        Low,
        Critical,
        Depleted // used for health to signal death
    }

    partial class TrackedBar
    {
        private Type componentType;
        private MemberInfo valueMemberInfo;
        private MemberInfo maxMemberInfo;
        internal Component component;
        internal BarState lastState;

        private const BindingFlags ALL = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        public bool Init(GameObject target)
        {
            componentType ??= ReflectionCache.GetType(componentTypeName);
            valueMemberInfo = componentType.GetMember(valueMemberName, ALL).FirstOrDefault();
            maxMemberInfo ??= componentType.GetMember(maxMemberName, ALL).FirstOrDefault();
            
            component = target.GetComponent(componentTypeName);
            return component;
        }

        public float GetValue() => GetMemberValue<float>(valueMemberInfo);
        public float GetMaxValue() => maxMemberInfo is null ? maxValue : GetMemberValue<float>(maxMemberInfo);
        public float GetPercentage() => GetValue() / GetMaxValue();

        private T GetMemberValue<T>(MemberInfo memberInfo)
        {
            return (T)(memberInfo switch
            {
                FieldInfo field => field.GetValue(component),
                PropertyInfo property => property.GetValue(component),
                MethodInfo getter when getter.GetParameters().Length == 0 => getter.Invoke(component, Array.Empty<object>()),
                _ => default(T),
            });
        }
    }

    public void FixedUpdate()
    {
        if (!Player.main) return;

        foreach (TrackedBar bar in trackedProperties)
        {
            if (!bar.component) bar.Init(Player.main.gameObject);
            if (bar.barName == "health" && UpdateDeath(bar)) continue;
            
            UpdateBar(bar);
        }
    }

    private bool UpdateDeath(TrackedBar healthBar)
    {
        LiveMixin liveMixin = (LiveMixin) healthBar.component;
        if (liveMixin.IsAlive()) return false;

        if (healthBar.lastState != BarState.Depleted)
        {
            healthBar.lastState = BarState.Depleted;
            NotifyDeath();
        }
        return true;
    }

    private void UpdateBar(TrackedBar bar)
    {
        if (!bar.component) return;

        float value = bar.GetPercentage();
        (float critical, float low) = (bar.thresholds.x, bar.thresholds.y);
        BarState state = value < critical ? BarState.Critical
            : value < low ? BarState.Low
            : BarState.Normal;

        if (state > bar.lastState)
            Notify(bar.barName, state);
        bar.lastState = state;
    }

    private void Notify(string property, BarState state)
    {
        SendTelemetry("react", $"{_coordinator.playerName}'s {property} is {FormatState(state)}."); // ({(int)(value*100)}%)
    }

    private void NotifyDeath()
    {
        SendTelemetry("react", $"{_coordinator.playerName} has died.");
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
