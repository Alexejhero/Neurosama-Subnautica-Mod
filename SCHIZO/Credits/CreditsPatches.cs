using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;

namespace SCHIZO.Credits;

[HarmonyPatch]
public static class CreditsPatches
{
    private static readonly Dictionary<string, string> _credits = new()
    {
        ["DrGlitchy"] = "The only guy who did anything",
        ["2Pfrog"] = "Can't type in chat",
        ["AlexejheroDev"] = "Can't do anything without Darkeewe",
        ["budwheizzah"] = "Kirby",
        ["Govorunb"] = "Has a weird pfp",

    };

    [HarmonyPatch(typeof(EndCreditsManager), nameof(EndCreditsManager.Start))]
    [HarmonyPostfix]
    public static void GetText(EndCreditsManager __instance)
    {
        easterEggAdjusted = false;

        float oldHeight = 14100;//__instance.textField.preferredHeight;
        __instance.textField.SetText(GetCreditsText() + __instance.textField.text);
        __instance.scrollSpeed = __instance.textField.preferredHeight * __instance.scrollSpeed / oldHeight;
        __instance.scrollStep = __instance.textField.preferredHeight * __instance.scrollStep / oldHeight;
    }

    private static string GetCreditsText()
    {
        StringBuilder builder = new("<style=h1>Neuro-sama Subnautica Mod</style>");
        builder.AppendLine();
        builder.AppendLine();

        foreach (KeyValuePair<string, string> credit in _credits)
        {
            builder.Append("<style=left>");
            builder.Append(credit.Key);
            builder.Append("</style>");
            builder.Append("<style=right>");
            builder.Append(credit.Value);
            builder.Append("</style>");
            builder.AppendLine();
        }

        builder.AppendLine();
        builder.AppendLine();

        return builder.ToString();
    }

    private static bool easterEggAdjusted;
    private static string[] sounds = Enumerable.Range(0, 4).Select(_ => Guid.NewGuid().ToString()).ToArray();
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
