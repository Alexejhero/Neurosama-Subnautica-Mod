using System;
using HarmonyLib;
using SCHIZO.Helpers;
using SCHIZO.Sounds.Players;

namespace SCHIZO.Sounds.Patches;

[HarmonyPatch]
public static class InventorySoundsPatches
{
    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    [HarmonyPostfix]
    public static void PlayInventorySounds()
    {
        foreach (InventoryItem item in Inventory.main.container.GetAllItems())
        {
            try
            {
                if (!item.item || item.item.gameObject.activeInHierarchy) continue;
                item.item.GetComponentsInChildren<InventoryAmbientSoundPlayer>()?.ForEach(c => c.Update());
            }
            catch (Exception e)
            {
                LOGGER.LogError($"Error while playing inventory sounds for {item.item.GetTechType()}: {e}");
            }
        }
    }
}
