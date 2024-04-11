using HarmonyLib;

namespace SCHIZO.Items.FumoItem;

internal static partial class FumoItemPatches
{
    [HarmonyPatch]
    public static class AllowKnifeStealIndoors
    {
        public static bool EnablePatch = false;

        [HarmonyPatch(typeof(Inventory), nameof(Inventory.CanDropItemHere))]
        [HarmonyPrefix]
        private static bool Prefix(ref bool __result)
        {
            if (!EnablePatch) return true;

            __result = true;
            return false;
        }
    }
}
