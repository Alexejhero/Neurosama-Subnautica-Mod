using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace SCHIZO;

[Menu("SCHIZO")]
public sealed class Config : ConfigFile
{
    [Toggle("Disable Ermfish inventory noises")]
    public bool DisableInventoryNoises = false;

    [Slider("Inventory noise minimum delay", 5, 300, DefaultValue = 60, Format = "{0:F0}", Step = 1)]
    public int MinInventoryNoiseDelay = 60;

    [Slider("Inventory noise maximum delay", 5, 300, DefaultValue = 120, Format = "{0:F0}", Step = 1)]
    public int MaxInventoryNoiseDelay = 120;

    [Toggle("Disable Erm/Ermfish world noises")]
    public bool DisableWorldNoises = false;

    [Slider("World noise minimum delay", 5, 60, DefaultValue = 10, Format = "{0:F0}", Step = 1)]
    public int MinWorldNoiseDelay = 10;

    [Slider("World noise maximum delay", 5, 60, DefaultValue = 30, Format = "{0:F0}", Step = 1)]
    public int MaxWorldNoiseDelay = 30;

    [Toggle("Disable all Ermfish noises")]
    public bool DisableAllNoises = false;

    /// <summary>
    /// Inverse scaling for the cooldown/nightly chance of a <see cref="Events.ErmMoonEvent"/> occurring.<br/>
    /// Scales approximately logarithmically. You can adjust the rates in <see cref="Events.ErmMoonEvent.Roll"/>
    /// </summary>
    // 0 - never, 1 - every 10-30 days, 3 - every 4-9 days, 6 - every 3-5 days, 9 - almost every night, 10 - guaranteed
    [Slider("Threat", 0, 10, DefaultValue = 3, Format = "{0:F0}", Step = 1)]
    public float MoonEventFrequency = 3;
}
