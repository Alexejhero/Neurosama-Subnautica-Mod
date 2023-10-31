using Nautilus.Options;
using Nautilus.Utility;
using UnityEngine.UI;

namespace SCHIZO.Options;

partial class ToggleModOption
{
    protected override bool ValueInternal
    {
        get => PlayerPrefsExtra.GetBool(PlayerPrefsKey, defaultValue);
        set => PlayerPrefsExtra.SetBool(PlayerPrefsKey, value);
    }

    protected override OptionItem GetOptionItemInternal()
    {
        ModToggleOption option = ModToggleOption.Create(id, label, Value, tooltip);
        option.OnChanged += (_, args) => Value = args.Value;

        OnValueChanged += value =>
        {
            if (option.OptionGameObject) option.OptionGameObject.GetComponentInChildren<Toggle>().SetIsOnWithoutNotify(value);
        };

        return option;
    }
}
