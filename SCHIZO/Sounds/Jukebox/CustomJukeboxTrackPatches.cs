using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using HarmonyLib;
using Nautilus.Utility;

namespace SCHIZO.Sounds.JukeboxButTheNamespaceConflictsWithTheGlobalJukeboxClassName.ThisIsWhyUnityIsTheBestGameEngine;

[HarmonyPatch]
public static class CustomJukeboxTrackPatches
{
    internal static readonly SelfCheckingDictionary<string, CustomJukeboxTrack> customTracks = new("");

    [HarmonyPatch(typeof(Jukebox), nameof(Jukebox.ScanInternal))]
    [HarmonyPostfix]
    public static void InsertCustomTracks(Jukebox __instance)
    {
        foreach (KeyValuePair<string, CustomJukeboxTrack> pair in customTracks)
        {
            __instance._playlist.Add(pair.Key);
            __instance._info[pair.Key] = pair.Value;
        }
    }

    [HarmonyPatch(typeof(Jukebox), nameof(Jukebox.UpdateLowLevel))]
    [HarmonyPrefix]
    public static bool AllowPlayingCustomTracks(Jukebox __instance)
    {
        if (!__instance.IsTrackCustom(out CustomJukeboxTrack track)) return true;

        if (track.SoundIsValid)
            return !track.isStream || UpdateStream(__instance);

        InitCustomTrack(__instance, track);
        return false;
    }

    private static void InitCustomTrack(Jukebox jukebox, CustomJukeboxTrack track)
    {
        if (!track.SoundIsValid)
        {
            if (track.IsRemote)
            {
                Jukebox.ERRCHECK(RuntimeManager.CoreSystem.createSound(track.URL, MODE._3D | MODE.CREATESTREAM | MODE.NONBLOCKING | MODE._3D_LINEARSQUAREROLLOFF, ref jukebox._exinfo, out track.sound));
            }
            else
            {
                track.sound = AudioUtils.CreateSound(track.audioClip, AudioUtils.StandardSoundModes_3D);
            }
        }
        jukebox._sound = track.sound;
        jukebox._info.Remove(track.identifier); // recalculate track info only once
    }

    [HarmonyPatch(typeof(Jukebox), nameof(Jukebox.UpdateInfo))]
    [HarmonyPrefix]
    public static bool DisplayInfoForCustomTracks(Jukebox __instance)
    {
        if (!__instance.IsTrackCustom(out CustomJukeboxTrack track)) return true;

        __instance._sound.getOpenState(out OPENSTATE state, out _, out _, out _);
        bool hasInfo = __instance._info.TryGetValue(track.identifier, out Jukebox.TrackInfo info);

        // streams can have their info change during playback
        bool assignOnce = track.IsLocal || track.overrideTrackLabel;
        if (hasInfo && assignOnce) return true;

        Jukebox.TrackInfo newInfo = (Jukebox.TrackInfo) track;
        if (assignOnce)
        {
            __instance.SetInfo(track.identifier, newInfo);
            return false;
        }

        if (state is OPENSTATE.READY or OPENSTATE.PLAYING)
        {
            __instance.TryGetArtistAndTitle(__instance._sound, out string artist, out string title);

            if (title is null or "") title = newInfo.label;
            newInfo.label = new TrackLabel() { artist = artist, title = title };

            if (info.label != newInfo.label)
                __instance.SetInfo(track.identifier, newInfo);
        }
        return false;
    }

    [HarmonyPatch(typeof(JukeboxInstance), nameof(JukeboxInstance.file), MethodType.Setter)]
    [HarmonyPostfix]
    public static void UnsetStreamInfoWhenSwitchingTracks(JukeboxInstance __instance)
    {
        if (!__instance.IsTrackCustom(out CustomJukeboxTrack track)) return;
        __instance.SetPositionKnobVisible(!track.isStream);
        if (track.isStream) __instance.SetLabel(track.trackLabel);
    }

    // the rest of the code in this class is dedicated to stopping FMOD/UWE from pausing or seeking the stream
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


public static class JukeboxExtensions
{
    public static bool IsTrackCustom(this Jukebox jukebox, out CustomJukeboxTrack track)
        => CustomJukeboxTrack.TryGetCustomTrack(jukebox._file, out track);

    public static bool IsTrackCustom(this JukeboxInstance jukebox, out CustomJukeboxTrack track)
        => CustomJukeboxTrack.TryGetCustomTrack(jukebox._file, out track);

    public static bool IsPlayingStream(this Jukebox jukebox)
        => IsTrackCustom(jukebox, out CustomJukeboxTrack track) && track.isStream;
    public static bool IsPlayingStream(this JukeboxInstance jukebox)
        => IsTrackCustom(jukebox, out CustomJukeboxTrack track) && track.isStream;
}
