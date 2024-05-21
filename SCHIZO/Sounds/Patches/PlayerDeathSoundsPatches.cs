using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SCHIZO.Helpers;
using SCHIZO.Sounds.Players;

namespace SCHIZO.Sounds.Patches;

[HarmonyPatch]
internal static class PlayerDeathSoundsPatches
{
    [HarmonyPatch(typeof(Player), nameof(Player.OnKill))]
    [HarmonyPostfix]
    public static void PlayDeathSound()
    {
        List<PlayerDeathSoundPlayer> deathSoundPlayers = Inventory.main.container.Select(item => item.item.gameObject)
            .SelectComponent<PlayerDeathSoundPlayer>()
            .ToList();
        if (deathSoundPlayers.Count == 0) return;
        PlayerDeathSoundPlayer player = deathSoundPlayers.Count == 1
            ? deathSoundPlayers[0]
            : deathSoundPlayers.GetRandom();
        player.PlayOneShot();
    }
}
