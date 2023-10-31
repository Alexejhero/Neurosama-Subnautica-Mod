using System.Collections.Generic;
using NaughtyAttributes;
using SCHIZO.Options.Bool;
using SCHIZO.Attributes.Visual;
using UnityEngine;

namespace SCHIZO.Options.Generic
{
    public abstract partial class ModOption : ScriptableObject
    {
        [Careful] public string id;

        [BoxGroup("Disable Conditions"), ReorderableList] public List<ToggleModOption> disableIfAnyTrue;
        [BoxGroup("Disable Conditions"), ReorderableList] public List<ToggleModOption> disableIfAnyFalse;
    }

    public abstract partial class ModOption<TValue> : ModOption where TValue : struct
    {
        public string label;
        public TValue defaultValue;
        [ResizableTextArea] public string tooltip;
    }

    public abstract partial class ModOption<TValue, TUpdater> : ModOption<TValue> where TValue : struct where TUpdater : OptionUpdater
    {
    }
}
