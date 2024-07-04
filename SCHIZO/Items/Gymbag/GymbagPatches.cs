using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SCHIZO.Items.Data;
using SCHIZO.Resources;

namespace SCHIZO.Items.Gymbag;

[HarmonyPatch]
public static class GymbagPatches
{
    private static readonly List<ItemData> _gymbagItems = [Assets.Mod_Gymbag_GymbagSN, Assets.Mod_Gymbag_GymbagBZ];
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
        if (!Inventory.main || !GymbagManager.Instance) return;

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

        if (!opener) return;

        if (opener.CurrentOpenedRootGymbag is { } && !opener.OpeningGymbag)
        {
            opener.SetChroma(opener.CurrentOpenedRootGymbag, 1);
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

    [HarmonyPatch(typeof(uGUI_InventoryTab), nameof(uGUI_InventoryTab.OnOpenPDA))]
    [HarmonyPostfix]
    public static void ModifyStorageLabel(uGUI_InventoryTab __instance)
    {
        GymbagManager opener = GymbagManager.Instance;

        if (!opener) return;

        if (opener.OpeningGymbag)
        {
            __instance.storageLabelKey = Gymbag.GymbagStorageLabel;
            __instance.UpdateStorageLabelText();
        }
    }
}
