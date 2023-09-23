using System;
using System.Linq;
using HarmonyLib;

namespace SCHIZO.Sounds;

[HarmonyPatch]
public static class SoundPatches
{
    [HarmonyPatch(typeof(Player), nameof(Player.Update))]
    [HarmonyPostfix]
    public static void PlayInventorySounds()
    {
        foreach (InventoryItem item in Inventory.main.container.GetItemTypes().SelectMany(Inventory.main.container.GetItems))
        {
            try
            {
                if (!item.item || item.item.gameObject.activeInHierarchy) continue;
                item.item.GetComponent<InventorySounds>()?.Update();
            }
            catch (Exception e)
            {
                LOGGER.LogError($"Error while playing inventory sounds for {item.item.GetTechType()}: {e}");
            }
        }
    }
}
