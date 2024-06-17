using System;
using Nautilus.Options;
using SCHIZO.Options.Generic;
using TMPro;
using UnityEngine;

namespace SCHIZO.Options.Button;

partial class ButtonModOption
{
    protected override OptionItem GetOptionItemInternal()
        => ModButtonOption.Create(id, label, (_) => onPressed.Invoke(), tooltip);

    public override Type GetOptionUpdaterType() => typeof(ButtonLabelUpdater);

    private class ButtonLabelUpdater : OptionUpdater
    {
        public override void UpdateOption()
        {
            base.UpdateOption();
            if (!gameObject) return;
            Transform labelTr = gameObject.transform.Find("Button/Label");
            if (!labelTr) return;
            TextMeshProUGUI label = labelTr.GetComponent<TextMeshProUGUI>();
            if (!label) return;
            label.text = modOption.label;
        }
    }
}
