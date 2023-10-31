using System.Collections.Generic;
using NaughtyAttributes;
using SCHIZO.Attributes.Visual;
using UnityEngine;

namespace SCHIZO.Options
{
    public abstract partial class ModOption<T> : ModOption where T : struct
    {
        public string label;
        public T defaultValue;
        [ResizableTextArea] public string tooltip;
    }

    public abstract partial class ModOption : ScriptableObject
    {
        [Careful] public string id;

        [BoxGroup("Disable Conditions"), ReorderableList] public List<ToggleModOption> disableIfAnyTrue;
        [BoxGroup("Disable Conditions"), ReorderableList] public List<ToggleModOption> disableIfAnyFalse;
    }
}
