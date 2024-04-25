using Nautilus.Handlers;
using SCHIZO.Attributes;

namespace SCHIZO.Tweaks;

public static class SeatruckSleeperModuleRename
{
    [InitializeMod]
    public static void Apply()
    {
        LanguageHandler.SetTechTypeName(TechType.SeaTruckSleeperModule, "Seatruck Jukebox Module");
        LanguageHandler.SetTechTypeName(TechType.SeaTruckSleeperModuleFragment, "Seatruck Jukebox Module Fragment");
        LanguageHandler.SetLanguageLine("Ency_SeaTruckSleeperModule", "Seatruck Jukebox Module");
        LanguageHandler.SetLanguageLine("PilotSeaTruckSleeperModule", "Pilot Jukebox Module");
    }
}
