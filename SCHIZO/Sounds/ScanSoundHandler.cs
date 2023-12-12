using System.Collections.Generic;
using HarmonyLib;
using SCHIZO.Helpers;

namespace SCHIZO.Sounds;

[HarmonyPatch]
public static class ScanSoundHandler
{
    private static readonly Dictionary<TechType, string> _scanSounds = [];

    public static void Register(TechType techType, string fmodEventPath)
    {
        _scanSounds[techType] = fmodEventPath;
    }

    [HarmonyPatch]
    public static class Patches
    {
        [HarmonyPatch(typeof(PDAScanner), nameof(PDAScanner.Unlock))]
        [HarmonyPostfix]
        public static void PlayCustomScanSound(PDAScanner.EntryData entryData, bool verbose)
        {
            if (!verbose) return; // prevents scan sounds playing on loading screen
            if (!_scanSounds.TryGetValue(entryData.key, out string sounds)) return;

            FMODHelpers.PlayPath2D(sounds);
        }
    }
}
