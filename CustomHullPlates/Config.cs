using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace SCHIZO
{
    [Menu("SCHIZO")]
    public sealed class Config : ConfigFile
    {
        [Toggle("Take some meds")]
        public bool DisableErmfishRandomNoises = false;

        [Toggle("Take more meds")]
        public bool DisableErmfishAllNoises = false;

        [Slider("Min SCHIZO time", 10, 600, DefaultValue = 60, Format = "{0:F0}s", Step = 10)]
        public float ErmfishMinRandomNoiseTime = 60;

        [Slider("Max SCHIZO time", 10, 600, DefaultValue = 60, Format = "{0:F0}s", Step = 10)]
        public float ErmfishMaxRandomNoiseTime = 300;
    }
}
