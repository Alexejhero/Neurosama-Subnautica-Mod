using System.Collections.Generic;
using HarmonyLib;
using SCHIZO.Sounds.Collections;

namespace SCHIZO.Sounds;

[HarmonyPatch]
public static class ScanSoundHandler
{
    private static readonly Dictionary<TechType, SoundCollectionInstance> _scanSounds = new();

    public static void Register(TechType techType, SoundCollectionInstance soundCollection)
    {
        _scanSounds.Add(techType, soundCollection);
    }

    [HarmonyPatch]
    public static class Patches
    {
        [HarmonyPatch(typeof(PDAScanner), nameof(PDAScanner.Unlock))]
        [HarmonyPostfix]
        public static void PlayCustomScanSound(PDAScanner.EntryData entryData, bool verbose)
        {
            if (!verbose) return; // prevents scan sounds playing on loading screen
            if (!_scanSounds.TryGetValue(entryData.key, out SoundCollectionInstance sounds)) return;

            sounds.PlayRandom2D();
        }
    }
}
