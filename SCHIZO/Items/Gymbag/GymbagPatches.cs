using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SCHIZO.Items.Data;
using SCHIZO.Resources.AssetBundles;

namespace SCHIZO.Items.Gymbag;

[HarmonyPatch]
public static class GymbagPatches
{
    private static readonly List<ItemData> _gymbagItems = new() {Assets.Gymbag_GymbagSN, Assets.Gymbag_GymbagBZ};
    private static bool IsGymbag(TechType type) => _gymbagItems.Any(t => t.ModItem == type);

    [HarmonyPatch(typeof(PickupableStorage), nameof(PickupableStorage.OnHandClick))]
    [HarmonyPrefix]
    public static bool AllowGymbagPickup(PickupableStorage __instance, GUIHand hand)
    {
        TechType type = __instance.pickupable.GetTechType();
        if (!IsGymbag(type)) return true;

        __instance.pickupable.OnHandClick(hand);
        return false;
    }

    [HarmonyPatch(typeof(PickupableStorage), nameof(PickupableStorage.OnHandHover))]
    [HarmonyPrefix]
    public static bool DisableGymbagPickupHudWarning(PickupableStorage __instance, GUIHand hand)
    {
        TechType type = __instance.pickupable.GetTechType();
        if (!IsGymbag(type)) return true;

        __instance.pickupable.OnHandHover(hand);
        return false;
    }

    [HarmonyPatch(typeof(uGUI_ItemsContainer), nameof(uGUI_ItemsContainer.Init))]
    [HarmonyPostfix]
    public static void SetInventoryUGUI(uGUI_ItemsContainer __instance, ItemsContainer container)
    {
        if (container == Inventory.main.container)
        {
            GymbagManager.Instance.InventoryUGUI = __instance;
        }
    }

    [HarmonyPatch(typeof(PDA), nameof(PDA.Close))]
    [HarmonyPostfix]
    public static void ClearLastOpenedOnPDAClose()
    {
        GymbagManager opener = GymbagManager.Instance;

        if (opener.CurrentOpenedRootGymbag != null && !opener.OpeningGymbag)
        {
            opener.GetItemIcon(opener.CurrentOpenedRootGymbag)?.SetChroma(1f);
            opener.CurrentOpenedRootGymbag.isEnabled = true;
            opener.CurrentOpenedRootGymbag = null;
        }
    }

    [HarmonyPatch(typeof(ItemsContainer), $"{nameof(IItemsContainer)}.{nameof(IItemsContainer.AllowedToRemove)}")]
    [HarmonyPrefix]
    public static bool PreventRemovingOpenedGymbag(ItemsContainer __instance, ref bool __result, Pickupable pickupable)
    {
        if (__instance != Inventory.main.container) return true;
        return __result = pickupable != GymbagManager.Instance.CurrentOpenedRootGymbag?.item;
    }
}
