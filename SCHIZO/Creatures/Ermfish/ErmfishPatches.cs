using Nautilus.Handlers;

namespace SCHIZO.Creatures.Ermfish;

[HarmonyPatch]
public static class ErmfishPatches
{
    [HarmonyPatch(typeof(LiveMixin), nameof(LiveMixin.Kill))]
    [HarmonyPostfix]
    public static void PlayPlayerDeathSound(LiveMixin __instance)
    {
        if (Player.main.liveMixin != __instance) return;
        if (ErmfishLoader.Instance.TechTypes.All(t => !Inventory.main.container.Contains(t))) return;
        ErmfishLoader.PlayerDeathSounds.Play2D(0.15f);
    }

    // TODO: Nautilus issue
    [HarmonyPatch(typeof(KnownTech), nameof(KnownTech.Initialize))]
    [HarmonyPostfix]
    public static void FixErmfishAnalysisTech()
    {
        if (KnownTech.analysisTech is null) return;

        KnownTech.AnalysisTech tech = KnownTech.analysisTech.FirstOrDefault(tech => tech.techType == ModItems.Ermfish);
        if (tech is null) return;
        tech.unlockMessage = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredMessage;
        tech.unlockSound = KnownTechHandler.DefaultUnlockData.NewCreatureDiscoveredSound;
    }
}
