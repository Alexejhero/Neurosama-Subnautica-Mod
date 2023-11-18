using HarmonyLib;
using Immersion.Trackers;

namespace Immersion.Patches;

[HarmonyPatch]
public static class SpikeyTrapPatches
{
    [HarmonyPatch(typeof(SpikeyTrapAttachTarget), nameof(SpikeyTrapAttachTarget.Attach))]
    [HarmonyPostfix]
    public static void NotifySpikeyTrapAttack(SpikeyTrapAttachTarget __instance)
    {
        if (__instance.player == Player.main)
        {
            PLUGIN_OBJECT.GetComponent<CreatureAttack>().NotifyAttackCinematic("Spikey Trap");
        }
    }
}
