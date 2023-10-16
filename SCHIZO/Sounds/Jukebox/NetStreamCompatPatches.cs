using FMOD.Studio;
using HarmonyLib;

namespace SCHIZO.Sounds.Jukebox_;

[HarmonyPatch]
public static class NetStreamCompatPatches
{
    // the code in this class is dedicated to stopping FMOD/UWE from pausing or seeking the stream
    // netstreams don't support it and will break and start spamming errors in the console and just generally making the game (and UWE's telemetry servers) not have a good time

    internal static bool UpdateStream(Jukebox jukebox)
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
        if (!__instance.IsPlayingStream()) return true;

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
        __instance.GetComponentInChildren<PointerEventTrigger>().enabled = !__instance.IsPlayingStream();
    }

    [HarmonyPatch(typeof(JukeboxInstance), nameof(JukeboxInstance.OnButtonPlayPause))]
    [HarmonyPrefix]
    public static bool DisablePauseButtonForHttpStreams(JukeboxInstance __instance)
    {
        return !(__instance.IsPlayingStream() && __instance.isControlling);
    }
}
