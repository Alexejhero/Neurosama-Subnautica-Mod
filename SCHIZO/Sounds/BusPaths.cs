using System;
using Nautilus.Utility;

namespace SCHIZO.Sounds;

public static class BusPathsExtensions
{
    public static string GetBusName(this BusPaths bus)
    {
        return bus switch
        {
            BusPaths.PDAVoice => AudioUtils.BusPaths.PDAVoice,
            BusPaths.UnderwaterCreatures => AudioUtils.BusPaths.UnderwaterCreatures,
            BusPaths.IndoorSounds => "bus:/master/SFX_for_pause/PDA_pause/all/indoorsounds",
            BusPaths.SFX => "bus:/master/SFX_for_pause/PDA_pause/all/SFX",
            BusPaths.SFXNoPDAPause => IS_SUBNAUTICA ? "bus:/master/SFX_for_pause/nofilter" : "bus:/master/SFX_for_pause",
            _ => throw new ArgumentOutOfRangeException(nameof(bus), bus, null)
        };
    }
}
