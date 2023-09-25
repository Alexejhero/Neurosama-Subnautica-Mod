using System;
using FMOD;
using FMODUnity;
using HarmonyLib;

namespace SCHIZO.Sounds.Radio;

[HarmonyPatch]
public static class JukeboxPatches
{
    [HarmonyPatch(typeof(Jukebox), nameof(Jukebox.ScanInternal))]
    [HarmonyPostfix]
    public static void InsertTruckersFM(Jukebox __instance)
    {
        __instance._playlist.Add("http://radio.truckers.fm/");
    }

    [HarmonyPatch(typeof(Jukebox), nameof(Jukebox.UpdateLowLevel))]
    [HarmonyPrefix]
    public static bool AllowPlayingHttpStreams(Jukebox __instance)
    {
        if (string.IsNullOrEmpty(__instance._file) || Jukebox.IsEvent(__instance._file))
            return true;

        if (!__instance._file.StartsWith("http")) return true;
        if (__instance._length > 0) __instance._length = 0;
        if (__instance._sound.hasHandle()) return true;

        CREATESOUNDEXINFO exInfo = __instance._exinfo with
        {
            pcmsetposcallback = (IntPtr sound, int subsound, uint position, TIMEUNIT postype) => RESULT.OK,
        };

        Jukebox.ERRCHECK(RuntimeManager.CoreSystem.createSound(__instance._file, MODE._3D | MODE.CREATESTREAM | MODE.NONBLOCKING | MODE._3D_LINEARSQUAREROLLOFF, ref __instance._exinfo, out __instance._sound));
        __instance.SetInfo(__instance._file, new() { label = "TruckersFM", length = 0 });
        __instance._instance?.SetPositionKnobVisible(false);
        
        return false;
    }

    [HarmonyPatch(typeof(Jukebox), nameof(Jukebox.UpdateInfo))]
    [HarmonyPrefix]
    public static bool DisplayInfoForHttpStreams(Jukebox __instance)
    {
        if (string.IsNullOrEmpty(__instance._file)) return true;
        if (!__instance._file.StartsWith("http")) return true;

        __instance._sound.getOpenState(out OPENSTATE state, out _, out _, out _);
        if (state is OPENSTATE.READY or OPENSTATE.PLAYING)
        {
            __instance.TryGetArtistAndTitle(__instance._sound, out string artist, out string title);
            string label = !string.IsNullOrEmpty(title)
                ? (!string.IsNullOrEmpty(artist) ? $"{artist} - {title}" : title)
                : "TruckersFM";
            if (!__instance._info.TryGetValue(__instance._file, out Jukebox.TrackInfo info) || info.label != label)
            {
                __instance.SetInfo(__instance._file, new() { label = label, length = 0 });
            }
        }
        return false;
    }

    // this is still necessary even with all the below patches
    // FMOD automatically pauses sounds that are outside the max falloff distance
    // (and netstreams can't handle pauses/seeking)
    [HarmonyPatch(typeof(Jukebox), nameof(Jukebox.ERRCHECK))]
    [HarmonyPrefix]
    public static bool AwfulHackToFixErrorSpamOnSeek()
    {
        if (!Jukebox._main) return true;
        if (!Jukebox._main._file?.StartsWith("http") ?? false) return true;

        // FMOD is still going to return errors to the game but it will be absolutely Clueless
        return false;
    }


    [HarmonyPatch(typeof(Jukebox), nameof(Jukebox.volume), MethodType.Setter)]
    [HarmonyPrefix]
    public static void DontFullyMuteBecauseItStopsPlayback(ref float value)
    {
        if (value == 0) value = 0.001f;
    }

    [HarmonyPatch(typeof(JukeboxInstance), nameof(JukeboxInstance.UpdateUI))]
    [HarmonyPostfix]
    public static void PreventSeekingHttpStreams(JukeboxInstance __instance)
    {
        bool isStream = __instance._file?.StartsWith("http") ?? false;
        __instance.GetComponentInChildren<PointerEventTrigger>().enabled = !isStream;
    }

    [HarmonyPatch(typeof(JukeboxInstance), nameof(JukeboxInstance.OnButtonPlayPause))]
    [HarmonyPrefix]
    public static bool PreventPausingHttpStreams(JukeboxInstance __instance)
    {
        return !((__instance._file?.StartsWith("http") ?? false) && __instance.isControlling);
    }
}
