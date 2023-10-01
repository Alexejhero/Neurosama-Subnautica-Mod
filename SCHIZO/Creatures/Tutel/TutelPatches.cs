using System.Linq;
using HarmonyLib;
using Nautilus.Handlers;

namespace SCHIZO.Creatures.Tutel;

[HarmonyPatch]
public static class TutelPatches
{
    // TODO: Nautilus issue
    [HarmonyPatch(typeof(KnownTech), nameof(KnownTech.Initialize))]
    [HarmonyPostfix]
    public static void FixTutelAnalysisTech()
    {
        if (KnownTech.analysisTech is null) return;

        KnownTech.AnalysisTech tech = KnownTech.analysisTech.FirstOrDefault(tech => tech.techType == ModItems.Tutel);
        if (tech is null) return;
        tech.unlockMessage = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredMessage;
        tech.unlockSound = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredSound;
    }
}
