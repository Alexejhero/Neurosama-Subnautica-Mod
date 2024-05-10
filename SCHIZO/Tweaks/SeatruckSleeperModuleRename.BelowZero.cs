using Nautilus.Handlers;
using SCHIZO.Attributes;

namespace SCHIZO.Tweaks;

public static class SeatruckSleeperModuleRename
{
    [InitializeMod]
    public static void Apply()
    {
        LanguageHandler.SetTechTypeName(TechType.SeaTruckSleeperModule, "Seatruck Entertainment Module");
        LanguageHandler.SetTechTypeName(TechType.SeaTruckSleeperModuleFragment, "Seatruck Entertainment Module Fragment");
        LanguageHandler.SetLanguageLine("Ency_SeaTruckSleeperModule", "Seatruck Entertainment Module");
        LanguageHandler.SetLanguageLine("PilotSeaTruckSleeperModule", "Pilot Entertainment Module");
    }
}
