using HarmonyLib;
using UnityEngine;

namespace SCHIZO.Patches;
[HarmonyPatch]
internal static class HolsterBeginPatches
{
    [HarmonyPatch(typeof(QuickSlots), nameof(QuickSlots.GetTransitionTime))]
    [HarmonyPostfix]
    public static void InjectBeginHolsterCall(QuickSlots __instance)
    {
        if (__instance.state != QuickSlots.ArmsState.Holster) return;
        InventoryItem item = __instance._heldItem;
        if (item is null || !item.item) return;

        // the funny
        item.item.gameObject.SendMessage("OnHolsterBegin", SendMessageOptions.DontRequireReceiver);
    }
}
