using FMOD.Studio;
using HarmonyLib;
using BZJukebox = Jukebox;

namespace SCHIZO.Sounds.Jukebox;

[HarmonyPatch]
public static class NetStreamCompatPatches
{
    // the code in this class is dedicated to stopping FMOD/UWE from pausing or seeking the stream
    // netstreams don't support it and will break and start spamming errors in the console and just generally making the game (and UWE's telemetry servers) not have a good time

    internal static bool UpdateStream(BZJukebox jukebox)
    {
        jukebox._length = 0;
        if (jukebox._paused)
        {
            jukebox.StopInternal();
            return false;
        }

        // manually stop playback instead of letting FMOD pause due to attenuation
        if (BZJukebox.instance && BZJukebox.instance.GetSoundPosition(out _, out float minDistance, out _)
            && minDistance > BZJukebox.maxDistance)
        {
            jukebox.StopInternal();
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(BZJukebox), nameof(BZJukebox.SetSnapshotState))]
    [HarmonyPrefix]
    public static bool DontMuteBecauseItPauses(BZJukebox __instance, EventInstance snapshot, ref bool state, bool value)
    {
        if (!__instance.IsPlayingStream()) return true;

#pragma warning disable Harmony003 // it's not an assignment... (remove when https://github.com/BepInEx/BepInEx.Analyzers/pull/6 is merged)
        return snapshot.handle != __instance.snapshotMute.handle;
#pragma warning restore Harmony003 // Harmony non-ref patch parameters modified
    }

    [HarmonyPatch(typeof(BZJukebox), nameof(BZJukebox.volume), MethodType.Setter)]
    [HarmonyPrefix]
    public static void PreventZeroVolumePause(ref float value)
    {
        if (value == 0) value = 0.001f;
    }

    [HarmonyPatch(typeof(JukeboxInstance), nameof(JukeboxInstance.UpdateUI))]
    [HarmonyPostfix]
    public static void DisableSeekBarForHttpStreams(JukeboxInstance __instance)
    {
        __instance.GetComponentInChildren<PointerEventTrigger>().enabled = !__instance.IsPlayingStream();
    }

    [HarmonyPatch(typeof(JukeboxInstance), nameof(JukeboxInstance.OnButtonPlayPause))]
    [HarmonyPrefix]
    public static bool DisablePauseButtonForHttpStreams(JukeboxInstance __instance)
    {
        return !(__instance.IsPlayingStream() && __instance.isControlling);
    }
}
