using System.Collections;
using FMOD;
using FMOD.Studio;
using HarmonyLib;
using UWE;
using BZJukebox = Jukebox;

namespace SCHIZO.Jukebox;

[HarmonyPatch]
public static class NetStreamCompatPatches
{
    // the code in this class is dedicated to stopping FMOD/UWE from pausing or seeking the stream
    // netstreams don't support it and WILL break

    internal static bool UpdateStream(BZJukebox jukebox, CustomJukeboxTrack track)
    {
        bool CanContinue()
        {
            if (!track.IsSoundValid(out OPENSTATE state)) return false;
            // fix error spam
            if (state == OPENSTATE.ERROR) return false;

            // can't pause streams
            if (jukebox._paused) return false;

            // manually stop playback instead of letting FMOD pause due to attenuation
            if (BZJukebox.instance && BZJukebox.instance.GetSoundPosition(out _, out float minDistance, out _)
                && minDistance > BZJukebox.maxDistance)
                return false;

            return true;
        }

        jukebox._length = 0;
        if (!CanContinue())
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
        if (!__instance.IsPlayingStream(out _)) return true;

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
    public static void AdjustUIForHttpStreams(JukeboxInstance __instance)
    {
        bool isStream = __instance.IsPlayingStream(out CustomJukeboxTrack track);
        __instance.GetComponentInChildren<PointerEventTrigger>().enabled = !isStream;

        if (!isStream) return;

        bool isPlaying = __instance.isControlling;
        bool hasInfo = BZJukebox.main._info.TryGetValue(track.identifier, out BZJukebox.TrackInfo info);

        if (isPlaying && (!hasInfo || info.label != __instance.textFile.text))
        {
            LOGGER.LogWarning($"Updating label because {(!hasInfo ? "no info" : $"{info.label} != {__instance.textFile.text}")}");
            __instance.SetLabel(isPlaying && hasInfo ? info.label : track.trackLabel);
        }
    }

    [HarmonyPatch(typeof(JukeboxInstance), nameof(JukeboxInstance.OnButtonPlayPause))]
    [HarmonyPrefix]
    public static bool DisablePauseButtonForHttpStreams(JukeboxInstance __instance)
    {
        if (!__instance.IsPlayingStream(out _)) return true;
        // isControlling essentially means "started playback on this jukebox"
        // if it's false, we're definitely not pausing
        if (!__instance.isControlling) return true;

        // usually we remove the label but since it's a "pause" we should keep it
        string labelHack = __instance.textFile.text;
        // not pause, but stop... it was lie! but the player never know.........
        BZJukebox.Stop();
        __instance.SetLabel(labelHack);
        return false;
    }

    [HarmonyPatch(typeof(IngameMenu), nameof(IngameMenu.OnDeselect))]
    [HarmonyPostfix]
    public static void AwfulAwfulPauseMenuHack()
    {
        // just... please pretend this isn't here
        // note: the proper solution to this is to download and buffer the netstream ourselves. however comma

        if (BZJukebox.main && BZJukebox.main.IsPlayingStream(out _) && BZJukebox.instance)
        {
            JukeboxInstance instance = BZJukebox.instance;
            BZJukebox.Stop();
            CoroutineHost.StartCoroutine(Bad(instance));
        }

        static IEnumerator Bad(JukeboxInstance instance)
        {
            BZJukebox.main.Release();
            yield return null;
            BZJukebox.Play(instance);
        }
    }
}
