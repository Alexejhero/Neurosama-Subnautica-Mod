using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using FMODUnity;
using HarmonyLib;
using Nautilus.Utility;
using UnityEngine;
using UWE;

namespace SCHIZO.Sounds.Jukebox_;

[HarmonyPatch]
public static class CustomJukeboxTrackPatches
{
    internal static readonly SelfCheckingDictionary<Jukebox.UnlockableTrack, CustomJukeboxTrack> customTracks = new("customTracks");

    internal static GameObject jukeboxDiskPrefab;

    [HarmonyPatch(typeof(Jukebox), nameof(Jukebox.Awake))]
    public static class AwakeWorkaround
    {
        // base Awake assumes all unlockable tracks are events
        // so we need to add ours afterwards (and remove them on game unload)
        [HarmonyPrefix]
        public static void ClearCustomTracks()
        {
            foreach (Jukebox.UnlockableTrack trackId in customTracks.Keys)
                Jukebox.unlockableMusic.Remove(trackId);
        }
        [HarmonyPostfix]
        public static void AddCustomTracks(Jukebox __instance)
        {
            foreach (KeyValuePair<Jukebox.UnlockableTrack, CustomJukeboxTrack> pair in customTracks)
            {
                CustomJukeboxTrack track = pair.Value;
                Jukebox.unlockableMusic[pair.Key] = track.identifier;
                Jukebox.musicLabels[track.identifier] = track.trackLabel; // only read inside Awake but why not
                __instance._info[track.identifier] = track;
            }
            if (!jukeboxDiskPrefab) CoroutineHost.StartCoroutine(GetJukeboxDiskPrefab());
        }
    }

    private static IEnumerator GetJukeboxDiskPrefab()
    {
        // const string diskClassId = "5108080f-242b-49e8-9b91-d01d6bbe138c";
        const string diskPrefabPath = "Misc/JukeboxDisk8.prefab";
        IPrefabRequest request = PrefabDatabase.GetPrefabForFilenameAsync(diskPrefabPath);
        yield return request;
        if (!request.TryGetPrefab(out jukeboxDiskPrefab))
            throw new Exception("Could not get prefab for jukebox disk!");
    }

    [HarmonyPatch(typeof(IntroVignette), nameof(IntroVignette.OnDone))]
    [HarmonyPostfix]
    public static void SetupUnlocksForCustomTracks()
    {
        // duplicate disks are not a problem - they self-destruct on Start if already unlocked
        customTracks.ForEach(pair => pair.Value.SetupUnlock(pair.Key));
    }

    [HarmonyPatch(typeof(Jukebox), nameof(Jukebox.ScanInternal))]
    [HarmonyPostfix]
    public static void RestoreCustomTrackInfoAfterScan(Jukebox __instance)
    {
        // base ScanInternal cleans _info of all non-event TrackInfo objects

        foreach (CustomJukeboxTrack track in customTracks.Values)
        {
            __instance._info[track.identifier] = track;
        }
    }

    [HarmonyPatch(typeof(Jukebox), nameof(Jukebox.UpdateLowLevel))]
    [HarmonyPrefix]
    public static bool AllowPlayingCustomTracks(Jukebox __instance)
    {
        if (!__instance.IsTrackCustom(out CustomJukeboxTrack track)) return true;

        if (track.IsSoundValid())
            return !track.isStream || NetStreamCompatPatches.UpdateStream(__instance);

        InitCustomTrack(__instance, track);
        return false;
    }

    private static void InitCustomTrack(Jukebox jukebox, CustomJukeboxTrack track)
    {
        if (!track.IsSoundValid())
        {
            if (track.IsRemote)
                Jukebox.ERRCHECK(RuntimeManager.CoreSystem.createSound(track.URL, MODE._3D | MODE.CREATESTREAM | MODE.NONBLOCKING | MODE._3D_LINEARSQUAREROLLOFF, ref jukebox._exinfo, out track.sound));
            else
                track.sound = AudioUtils.CreateSound(track.audioClip, AudioUtils.StandardSoundModes_3D);
        }
        jukebox._sound = track.sound;
    }

    [HarmonyPatch(typeof(Jukebox), nameof(Jukebox.HandleOpenError))]
    [HarmonyPrefix]
    public static void RemoveErroringTrack(Jukebox __instance)
    {
        if (!__instance.IsTrackCustom(out CustomJukeboxTrack track)) return;

        LOGGER.LogError($"Could not load track '{track.identifier}' from {track.URL}, removing track from playlist");
        __instance._playlist.Remove(track.identifier);
        Jukebox.UnlockableTrack trackId = track;
        Jukebox.unlockableMusic.Remove(trackId);
        Player.main.unlockedTracks.Remove(trackId);
        // customTracks.Remove(trackId);
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
            //LOGGER.LogWarning($"Set info from asset - ({newInfo.label},{newInfo.length})");
            __instance.SetInfo(track.identifier, newInfo);
            return false;
        }

        if (state is OPENSTATE.READY or OPENSTATE.PLAYING)
        {
            __instance.TryGetArtistAndTitle(__instance._sound, out string artist, out string title);
            if (!track.isStream) __instance._sound.getLength(out newInfo.length, TIMEUNIT.MS);

            if (title is null or "") title = newInfo.label;
            newInfo.label = new TrackLabel() { artist = artist, title = title };

            if (info.label != newInfo.label)
            {
                //LOGGER.LogWarning($"Set info from remote - ({newInfo.label},{newInfo.length})");
                __instance.SetInfo(track.identifier, newInfo);
            }
        }
        return false;
    }

    [HarmonyPatch(typeof(JukeboxInstance), nameof(JukeboxInstance.file), MethodType.Setter)]
    [HarmonyPostfix]
    public static void OnSwitchingTracks(JukeboxInstance __instance)
    {
        if (!__instance.IsTrackCustom(out CustomJukeboxTrack track)) return;

        // can't seek streams
        __instance.SetPositionKnobVisible(!track.isStream);
        // reset track info
        __instance.SetLabel(track.trackLabel);
        uint length = track.Length;
        if (length > 0) __instance.SetLength(length);
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
