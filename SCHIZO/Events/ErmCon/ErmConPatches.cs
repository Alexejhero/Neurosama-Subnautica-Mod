using HarmonyLib;
using SCHIZO.Creatures.Ermfish;

namespace SCHIZO.Events.ErmCon;

[HarmonyPatch]
public static class ErmConPatches
{
    [HarmonyPatch(typeof(Pickupable), nameof(Pickupable.OnHandClick))]
    [HarmonyPrefix]
    public static bool PreventErmfishPickup(Pickupable __instance, GUIHand hand)
    {
        TechType type = __instance.GetTechType();
        if (!ErmfishLoader.ErmfishTechTypes.Contains(type)) return true;

        return !CustomEventManager.main!?.GetEvent<ErmConEvent>()!?.IsOccurring ?? true;
    }

    [HarmonyPatch(typeof(Pickupable), nameof(Pickupable.OnHandHover))]
    [HarmonyPrefix]
    public static bool PreventErmfishPickupWarning(Pickupable __instance, GUIHand hand)
    {
        TechType type = __instance.GetTechType();
        if (!ErmfishLoader.ErmfishTechTypes.Contains(type)) return true;

        if (!CustomEventManager.main!?.GetEvent<ErmConEvent>()!?.IsOccurring ?? true) return true;

        HandReticle.main.SetText(HandReticle.TextType.Hand, "Cannot pick up", false);
        HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "Inappropriate conduct is forbidden during ErmCon", false);

        return false;
    }
}
