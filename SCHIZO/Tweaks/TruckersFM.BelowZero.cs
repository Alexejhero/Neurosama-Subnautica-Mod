using FMOD;
using FMOD.Studio;
using FMODUnity;

namespace SCHIZO.Tweaks;

[HarmonyPatch]
public static class TruckersFM
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

        if (__instance._sound.hasHandle()) return UpdateStream(__instance);

        InitStream(__instance);
        return false;
    }

    private static void InitStream(Jukebox jukebox)
    {
        Jukebox.ERRCHECK(RuntimeManager.CoreSystem.createSound(jukebox._file, MODE._3D | MODE.CREATESTREAM | MODE.NONBLOCKING | MODE._3D_LINEARSQUAREROLLOFF, ref jukebox._exinfo, out jukebox._sound));
        jukebox.SetInfo(jukebox._file, new() { label = "TruckersFM", length = 0 });
        jukebox._instance!?.SetPositionKnobVisible(false);
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

    // the rest of the code in this file is dedicated to stopping FMOD/UWE from pausing or seeking the stream
    // netstreams don't support it and will break and start spamming errors in the console and just generally making the game (and UWE's telemetry servers) not have a good time

    private static bool UpdateStream(Jukebox jukebox)
    {
        jukebox._length = 0;
        if (jukebox._paused)
        {
            jukebox.StopInternal();
            return false;
        }

        // manually stop playback instead of letting FMOD pause due to attenuation
        if (Jukebox.instance && Jukebox.instance.GetSoundPosition(out _, out float minDistance, out _)
            && minDistance > Jukebox.maxDistance)
        {
            jukebox.StopInternal();
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(Jukebox), nameof(Jukebox.SetSnapshotState))]
    [HarmonyPrefix]
    public static bool DontMuteBecauseItPauses(Jukebox __instance, EventInstance snapshot, ref bool state, bool value)
    {
        if (string.IsNullOrEmpty(__instance._file)) return true;
        if (!__instance._file.StartsWith("http")) return true;

#pragma warning disable Harmony003 // it's not an assignment... (remove when https://github.com/BepInEx/BepInEx.Analyzers/pull/6 is merged)
        return snapshot.handle != __instance.snapshotMute.handle;
#pragma warning restore Harmony003 // Harmony non-ref patch parameters modified
    }

    [HarmonyPatch(typeof(Jukebox), nameof(Jukebox.volume), MethodType.Setter)]
    [HarmonyPrefix]
    public static void PreventZeroVolumePause(ref float value)
    {
        if (value == 0) value = 0.001f;
    }

    [HarmonyPatch(typeof(JukeboxInstance), nameof(JukeboxInstance.UpdateUI))]
    [HarmonyPostfix]
    public static void DisableSeekBarForHttpStreams(JukeboxInstance __instance)
    {
        bool isStream = __instance._file?.StartsWith("http") ?? false;
        __instance.GetComponentInChildren<PointerEventTrigger>().enabled = !isStream;
    }

    [HarmonyPatch(typeof(JukeboxInstance), nameof(JukeboxInstance.OnButtonPlayPause))]
    [HarmonyPrefix]
    public static bool DisablePauseButtonForHttpStreams(JukeboxInstance __instance)
    {
        return !((__instance._file?.StartsWith("http") ?? false) && __instance.isControlling);
    }
}
