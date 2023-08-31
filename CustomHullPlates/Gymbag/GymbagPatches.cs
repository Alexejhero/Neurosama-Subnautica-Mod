using HarmonyLib;

namespace SCHIZO.Gymbag;

[HarmonyPatch]
public static class GymbagPatches
{
    [HarmonyPatch(typeof(PickupableStorage), nameof(PickupableStorage.OnHandClick))]
    [HarmonyPrefix]
    public static bool AllowGymbagPickup(PickupableStorage __instance, GUIHand hand)
    {
        TechType type = __instance.pickupable.GetTechType();
        if (type != GymbagTypes.Gymbag.TechType) return true;

        __instance.pickupable.OnHandClick(hand);
        return false;
    }

    [HarmonyPatch(typeof(PickupableStorage), nameof(PickupableStorage.OnHandHover))]
    [HarmonyPrefix]
    public static bool DisableGymbagPickupHudWarning(PickupableStorage __instance, GUIHand hand)
    {
        TechType type = __instance.pickupable.GetTechType();
        if (type != GymbagTypes.Gymbag.TechType) return true;

        __instance.pickupable.OnHandHover(hand);
        return false;
    }

    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    [HarmonyPostfix]
    public static void AddGymbagHandler(Player __instance)
    {
        __instance.gameObject.EnsureComponent<GymbagHandler>();
    }

    [HarmonyPatch(typeof(uGUI_ItemsContainer), nameof(uGUI_ItemsContainer.Init))]
    [HarmonyPostfix]
    public static void SetInventoryUGUI(uGUI_ItemsContainer __instance, ItemsContainer container)
    {
        if (container == Inventory.main.container)
        {
            GymbagHandler.main.InventoryUGUI = __instance;
        }
    }

    [HarmonyPatch(typeof(PDA), nameof(PDA.Close))]
    [HarmonyPostfix]
    public static void ClearLastOpenedOnPDAClose()
    {
        GymbagHandler opener = GymbagHandler.main;

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
        return __result = pickupable != GymbagHandler.main.CurrentOpenedRootGymbag?.item;
    }
}
