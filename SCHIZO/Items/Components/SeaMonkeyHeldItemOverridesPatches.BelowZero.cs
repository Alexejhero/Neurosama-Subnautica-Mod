using HarmonyLib;

namespace SCHIZO.Items.Components;
[HarmonyPatch]
public static class SeaMonkeyHeldItemOverridesPatches
{
    [HarmonyPatch(typeof(SeaMonkeyHeldItem), nameof(SeaMonkeyHeldItem.Hold), [typeof(Pickupable), typeof(bool)])]
    [HarmonyPostfix]
    public static void OnHold(Pickupable pickupable)
    {
        if (!pickupable) return;
        SeaMonkeyHeldItemOverrides overrideComponent = pickupable.GetComponent<SeaMonkeyHeldItemOverrides>();
        if (!overrideComponent) return;

        overrideComponent.OnPickedUp();
    }

    [HarmonyPatch(typeof(SeaMonkeyHeldItem), nameof(SeaMonkeyHeldItem.Drop))]
    [HarmonyPrefix]
    public static void OnDrop(SeaMonkeyHeldItem __instance)
    {
        if (!__instance.item) return;
        SeaMonkeyHeldItemOverrides overrideComponent = __instance.item.GetComponent<SeaMonkeyHeldItemOverrides>();
        if (!overrideComponent) return;

        overrideComponent.OnDropped();
    }
}
