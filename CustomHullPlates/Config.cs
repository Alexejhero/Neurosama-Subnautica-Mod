using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace SCHIZO
{
    [Menu("SCHIZO")]
    public sealed class Config : ConfigFile
    {
        [Toggle("Take some meds")]
        public bool DisableInventoryNoises = false;

        [Slider("SCHIZO slider min", 5, 300, DefaultValue = 60, Format = "{0:F0}", Step = 1)]
        public float MinInventoryNoiseDelay = 60;

        [Slider("SCHIZO slider max", 5, 300, DefaultValue = 120, Format = "{0:F0}", Step = 1)]
        public float MaxInventoryNoiseDelay = 120;

        [Toggle("Take other meds")]
        public bool DisableWorldNoises = false;

        [Slider("SCHIZO slider min 2", 5, 60, DefaultValue = 10, Format = "{0:F0}", Step = 1)]
        public float MinWorldNoiseDelay = 10;

        [Slider("SCHIZO slider max 2", 5, 60, DefaultValue = 30, Format = "{0:F0}", Step = 1)]
        public float MaxWorldNoiseDelay = 30;

        [Toggle("Take all meds")]
        public bool DisableAllNoises = false;
    }
}
