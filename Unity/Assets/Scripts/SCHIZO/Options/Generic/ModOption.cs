using System.Collections.Generic;
using SCHIZO.Attributes;
using SCHIZO.Options.Bool;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Options.Generic
{
    [DeclareBoxGroup("disabling", Title = "Disable Conditions")]
    public abstract partial class ModOption : ScriptableObject
    {
        [Careful] public string id;

        [Group("disabling"), ListDrawerSettings] public List<ToggleModOption> disableIfAnyTrue;
        [Group("disabling"), ListDrawerSettings] public List<ToggleModOption> disableIfAnyFalse;
    }

    public abstract partial class ModOption<TValue> : ModOption where TValue : struct
    {
        public string label;
        public TValue defaultValue;
        [TextArea(1, 5)] public string tooltip;
    }

    // ReSharper disable once UnusedTypeParameter
    public abstract partial class ModOption<TValue, TUpdater> : ModOption<TValue> where TValue : struct where TUpdater : OptionUpdater
    {
    }
}
