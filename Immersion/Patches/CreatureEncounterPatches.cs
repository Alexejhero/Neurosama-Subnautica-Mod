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

    private static readonly Dictionary<string, TechType> _cinematics = new() {
        ["squidshark_release"] = TechType.SquidShark,
        ["squidshark_kill"] = TechType.SquidShark,
        ["player_attack"] = TechType.SnowStalker, // this is indeed the only anim with that name
        ["player_attack_roll"] = TechType.SnowStalker,
        ["player_kill"] = TechType.SnowStalker,
        ["chelicerate_player_attack"] = TechType.Chelicerate, // kills outside of vehicles
        ["shadowLevi_player_attack"] = TechType.ShadowLeviathan, // very much kills
    };

    [HarmonyPatch(typeof(PlayerCinematicController), nameof(PlayerCinematicController.StartCinematicMode))]
    [HarmonyPostfix]
    public static void NotifyCinematicAttack(PlayerCinematicController __instance, Player setplayer)
    {
        if (_cinematics.TryGetValue(__instance.playerViewAnimationName, out var techType))
            Encounters?.NotifyCreatureEncounter(techType);
    }

    //[HarmonyPatch(typeof(PlayerCinematicController), nameof(PlayerCinematicController.Start))]
    //[HarmonyPostfix]
    //public static void bluh(PlayerCinematicController __instance)
    //{
    //    __instance.debug = true;
    //}
}
