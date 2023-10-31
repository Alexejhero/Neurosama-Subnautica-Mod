using System;

namespace SCHIZO.Options
{
    [Serializable]
    public sealed partial class ConfigurableValueFloat : ConfigurableValue<float, SliderModOption>
    {
        public enum CalculateMode
        {
            OneOf,
            Min,
            Max
        }

        public CalculateMode calculateMode = CalculateMode.OneOf;

        protected override bool ShowIsHardCoded => calculateMode == CalculateMode.OneOf;
        protected override bool ShowModOption => calculateMode != CalculateMode.OneOf || !isHardCoded;
        protected override bool ShowValue => calculateMode != CalculateMode.OneOf || isHardCoded;
    }
}
