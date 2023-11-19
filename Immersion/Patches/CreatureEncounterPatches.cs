using HarmonyLib;
using Immersion.Trackers;

namespace Immersion.Patches;

[HarmonyPatch]
public static class CreatureEncounterPatches
{
    private static CreatureEncounters _encounters;
                                                    // unity lifetime checks smile (also rider L)
    private static CreatureEncounters Encounters => _encounters ? _encounters : _encounters = PLUGIN_OBJECT.GetComponent<CreatureEncounters>();

    [HarmonyPatch(typeof(SpikeyTrapAttachTarget), nameof(SpikeyTrapAttachTarget.Attach))]
    [HarmonyPostfix]
    public static void NotifySpikeyTrapAttack(SpikeyTrapAttachTarget __instance)
    {
        if (__instance.player != Player.main) return;
        
        Encounters.NotifyCreatureEncounter(TechType.SpikeyTrap);
    }

    [HarmonyPatch(typeof(PlayerLilyPaddlerHypnosis), nameof(PlayerLilyPaddlerHypnosis.StartHypnosis))]
    [HarmonyPostfix]
    public static void NotifyLilyPaddlerHypnosis()
    {
        Encounters.NotifyCreatureEncounter(TechType.LilyPaddler);
    }
}
