using Immersion.Trackers;
using Nautilus.Extensions;

namespace Immersion.Patches;

[HarmonyPatch]
public static class CreatureEncounterPatches
{
#nullable enable
    private static CreatureEncounters? Encounters => COMPONENT_HOLDER.GetComponent<CreatureEncounters>().Exists();

    [HarmonyPatch(typeof(SpikeyTrapAttachTarget), nameof(SpikeyTrapAttachTarget.Attach))]
    [HarmonyPostfix]
    public static void NotifySpikeyTrapAttack(SpikeyTrapAttachTarget __instance)
    {
        if (__instance.player != Player.main) return;

        Encounters?.NotifyCreatureEncounter(TechType.SpikeyTrap);
    }

    [HarmonyPatch(typeof(PlayerLilyPaddlerHypnosis), nameof(PlayerLilyPaddlerHypnosis.StartHypnosis))]
    [HarmonyPostfix]
    public static void NotifyLilyPaddlerHypnosis()
    {
        Encounters?.NotifyCreatureEncounter(TechType.LilyPaddler);
    }

    [HarmonyPatch(typeof(IceWormJumpScareTrigger), nameof(IceWormJumpScareTrigger.InvokeJumpScareEvent))]
    [HarmonyPostfix]
    public static void NotifyIceWormJumpScare(IceWormJumpScareTrigger __instance)
    {
        if (!__instance.used) return;

        Encounters?.NotifyCreatureEncounter(TechType.IceWorm);
    }
}
