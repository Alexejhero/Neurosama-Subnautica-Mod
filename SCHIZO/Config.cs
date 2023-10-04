using JetBrains.Annotations;
using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace SCHIZO;

[Menu("SCHIZO")]
public sealed class Config : ConfigFile
{
    [Toggle("Disable inventory sounds"), UsedImplicitly]
    public bool DisableInventoryNoises = false;

    [Slider("Inventory sounds minimum delay", 5, 300, DefaultValue = 60, Format = "{0:F0}", Step = 1), UsedImplicitly]
    public int MinInventoryNoiseDelay = 60;

    [Slider("Inventory sounds maximum delay", 5, 300, DefaultValue = 120, Format = "{0:F0}", Step = 1), UsedImplicitly]
    public int MaxInventoryNoiseDelay = 120;

    [Toggle("Disable world sounds"), UsedImplicitly]
    public bool DisableWorldNoises = false;

    [Slider("World sounds minimum delay", 5, 60, DefaultValue = 10, Format = "{0:F0}", Step = 1), UsedImplicitly]
    public int MinWorldNoiseDelay = 10;

    [Slider("World sounds maximum delay", 5, 60, DefaultValue = 30, Format = "{0:F0}", Step = 1), UsedImplicitly]
    public int MaxWorldNoiseDelay = 30;

    [Toggle("Disable all sounds"), UsedImplicitly]
    public bool DisableAllNoises = false;
}
