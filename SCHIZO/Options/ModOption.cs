using System;
using System.Collections.Generic;
using Nautilus.Handlers;
using Nautilus.Options;
using UnityEngine;

namespace SCHIZO.Options;

partial class ModOption<T>
{
    protected string PlayerPrefsKey => $"SCHIZO_ModOption_{id}";

    public T Value
    {
        get => ValueInternal;
        set
        {
            ValueInternal = value;
            OnValueChanged?.Invoke(value);
        }
    }
    protected abstract T ValueInternal { get; set; }

    public event Action<T> OnValueChanged;
}

partial class ModOption
{
    public static Dictionary<OptionItem, ModOption> OptionItems { get; } = new();

    public OptionItem GetOptionItem()
    {
        OptionItem result = GetOptionItemInternal();
        OptionItems[result] = this;
        return result;
    }

    protected abstract OptionItem GetOptionItemInternal();

    public virtual void AddRealtimeUpdater(GameObject optionObject)
    {
        RealtimeOptionUpdater updater = optionObject.AddComponent<RealtimeOptionUpdater>();
        updater.modOption = this;
        updater.OnEnable();

        foreach (ToggleModOption toggleModOption in disableIfAnyFalse)
        {
            toggleModOption.OnValueChanged += _ =>
            {
                if (updater) updater.OnEnable();
            };
        }

        foreach (ToggleModOption toggleModOption in disableIfAnyTrue)
        {
            toggleModOption.OnValueChanged += _ =>
            {
                if (updater) updater.OnEnable();
            };
        }
    }
}
