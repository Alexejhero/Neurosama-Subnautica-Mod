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
        Pickupable item = __instance._heldItem?.item;
        if (!item) return;

        // the funny
        item.gameObject.SendMessage("OnHolsterBegin", SendMessageOptions.DontRequireReceiver);
    }
}
