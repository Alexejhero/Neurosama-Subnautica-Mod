using System;
using System.Collections.Generic;
using Nautilus.Options;
using SCHIZO.Options.Bool;

namespace SCHIZO.Options.Generic;

partial class ModOption
{
    public static Dictionary<OptionItem, ModOption> OptionItems { get; } = [];

    public OptionItem GetOptionItem()
    {
        OptionItem result = GetOptionItemInternal();
        OptionItems[result] = this;
        return result;
    }
    protected abstract OptionItem GetOptionItemInternal();

    public virtual void SetupOptionUpdater(OptionUpdater updater)
    {
        foreach (ToggleModOption toggleModOption in disableIfAnyFalse)
        {
            toggleModOption.OnValueChanged += _ =>
            {
                if (updater) updater.UpdateOption();
            };
        }

        foreach (ToggleModOption toggleModOption in disableIfAnyTrue)
        {
            toggleModOption.OnValueChanged += _ =>
            {
                if (updater) updater.UpdateOption();
            };
        }
    }

    public virtual Type GetOptionUpdaterType() => typeof(OptionUpdater);
}

partial class ModOption<TValue>
{
    protected string PlayerPrefsKey => $"SCHIZO_ModOption_{id}";

    public TValue Value
    {
        get => ValueInternal;
        set
        {
            ValueInternal = value;
            OnValueChanged?.Invoke(value);
        }
    }
    protected abstract TValue ValueInternal { get; set; }

    public event Action<TValue> OnValueChanged;
}

partial class ModOption<TValue, TUpdater>
{
    public sealed override void SetupOptionUpdater(OptionUpdater updater)
    {
        base.SetupOptionUpdater(updater);
        SetupOptionUpdaterInternal((TUpdater) updater);
    }
    protected abstract void SetupOptionUpdaterInternal(TUpdater updater);

    public sealed override Type GetOptionUpdaterType() => typeof(TUpdater);
}
