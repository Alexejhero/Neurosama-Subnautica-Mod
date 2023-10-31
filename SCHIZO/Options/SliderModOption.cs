using Nautilus.Options;
using UnityEngine;

namespace SCHIZO.Options;

partial class SliderModOption
{
    protected override float ValueInternal
    {
        get => PlayerPrefs.GetFloat(PlayerPrefsKey, defaultValue);
        set => PlayerPrefs.SetFloat(PlayerPrefsKey, value);
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
}
