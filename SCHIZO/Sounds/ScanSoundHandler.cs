using System.Collections.Generic;
using HarmonyLib;
using Nautilus.Utility;
using SCHIZO.Sounds.Collections;

namespace SCHIZO.Sounds;

[HarmonyPatch]
public static class ScanSoundHandler
{
    private static readonly Dictionary<TechType, SoundCollectionInstance> _scanSounds = new();

    public static void Register(TechType techType, SoundCollection soundCollection)
    {
        _scanSounds.Add(techType, soundCollection.Initialize(AudioUtils.BusPaths.PDAVoice));
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

            sounds.Play2D();
        }
    }
}
