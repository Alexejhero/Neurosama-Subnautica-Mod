using System;
using TriInspector;

namespace SCHIZO.Options.Generic
{
    [Serializable]
    public abstract partial class ConfigurableValue<TRaw, TModOption> where TRaw : struct where TModOption : ModOption<TRaw>
    {
        [ShowIf(nameof(ShowIsHardCoded))] public bool isHardCoded = false;
        [ShowIf(nameof(ShowModOption)), Required] public TModOption modOption;
        [ShowIf(nameof(ShowValue))] public TRaw value;

        protected virtual bool ShowIsHardCoded => true;
        protected virtual bool ShowModOption => !isHardCoded;
        protected virtual bool ShowValue => isHardCoded;
    }
}
