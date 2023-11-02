using Nautilus.Options;
using UnityEngine;

namespace SCHIZO.Options.Float;

partial class SliderModOption
{
    protected override float ValueInternal
    {
        get => PlayerPrefs.GetFloat(PlayerPrefsKey, defaultValue);
        set => PlayerPrefs.SetFloat(PlayerPrefsKey, Clamped(value));
    }

    protected override OptionItem GetOptionItemInternal()
    {
        ModSliderOption option = ModSliderOption.Create(id, label, min, max, Value, defaultValue, valueFormat, step, tooltip);
        option.OnChanged += (_, args) => Value = args.Value;

        OnValueChanged += value =>
        {
            if (option.OptionGameObject) option.OptionGameObject.GetComponentInChildren<uGUI_SnappingSlider>().SetValueWithoutNotify(value);
        };

        return option;
    }

    private float Clamped(float value)
    {
        return Mathf.Clamp(value, min, max);
    }
}
