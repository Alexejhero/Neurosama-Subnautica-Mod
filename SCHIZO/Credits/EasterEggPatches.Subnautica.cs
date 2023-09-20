using System;
using System.Collections;
using System.IO;
using System.Linq;
using HarmonyLib;
using Nautilus.Handlers;
using UnityEngine;

namespace SCHIZO.Credits;

public static class EasterEggPatches
{
    public static bool easterEggAdjusted;

    private static readonly string[] sounds = Enumerable.Range(0, 4).Select(_ => Guid.NewGuid().ToString()).ToArray();
    [HarmonyPatch(typeof(EndCreditsManager), nameof(EndCreditsManager.OnLateUpdate))]
    [HarmonyPostfix]
    public static void OnLateUpdate(EndCreditsManager __instance)
    {
        if (__instance.phase != EndCreditsManager.Phase.Easter || EndCreditsManager.showEaster) return;

        if (!easterEggAdjusted)
        {
            if (!CustomSoundHandler.TryGetCustomSound(sounds[0], out _))
            {
                CustomSoundHandler.RegisterCustomSound(sounds[0], Path.Combine(AssetLoader.AssetsFolder, "sounds", "ermfish", "noises", "well.mp3"), "bus:/master/SFX_for_pause/nofilter");
                CustomSoundHandler.RegisterCustomSound(sounds[1], Path.Combine(AssetLoader.AssetsFolder, "sounds", "tutel", "noises", "vedal_yeah_clean.mp3"), "bus:/master/SFX_for_pause/nofilter");
                CustomSoundHandler.RegisterCustomSound(sounds[2], Path.Combine(AssetLoader.AssetsFolder, "sounds", "ermfish", "noises", "neuro-ermcon.mp3"), "bus:/master/SFX_for_pause/nofilter");
                CustomSoundHandler.RegisterCustomSound(sounds[3], Path.Combine(AssetLoader.AssetsFolder, "sounds", "tutel", "hurt", "vedal_nooooooooo.mp3"), "bus:/master/SFX_for_pause/nofilter");
            }

            GameInput.instance.StartCoroutine(PlayAt(sounds[0], __instance.phaseStartTime));
            GameInput.instance.StartCoroutine(PlayAt(sounds[1], __instance.phaseStartTime + 1.5f));
            GameInput.instance.StartCoroutine(PlayAt(sounds[2], __instance.phaseStartTime + 3));
            GameInput.instance.StartCoroutine(PlayAt(sounds[3], __instance.phaseStartTime + 3.5f));

            // it's actually the end time (just for the easter egg phase)
            __instance.phaseStartTime += 5f;

            easterEggAdjusted = true;
        }
    }

    private static IEnumerator PlayAt(string fmodAssetId, float time = 0f)
    {
        float delay = time - Time.unscaledTime;
        if (delay > 0) yield return new WaitForSecondsRealtime(delay);

        CustomSoundHandler.TryPlayCustomSound(fmodAssetId, out _);
    }
}
