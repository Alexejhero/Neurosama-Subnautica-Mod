using HarmonyLib;
using SCHIZO.Sounds.Players;
using UnityEngine;

namespace SCHIZO.Sounds.Patches;

[HarmonyPatch]
internal static class PlayerDeathSoundsPatches
{
    [HarmonyPatch(typeof(Player), nameof(Player.OnKill))]
    [HarmonyPostfix]
    public static void PlayDeathSound()
    {
        foreach (PlayerDeathSoundPlayer sndPlayer in UnityEngine.Resources.FindObjectsOfTypeAll<PlayerDeathSoundPlayer>())
        {
            sndPlayer.Play(Random.Range(0, 0.5f));
        }
    }
}
