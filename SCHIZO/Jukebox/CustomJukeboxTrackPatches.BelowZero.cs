using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using FMODUnity;
using HarmonyLib;
using JetBrains.Annotations;
using Nautilus.Utility;
using UnityEngine;
using UWE;
using BZJukebox = Jukebox;

namespace SCHIZO.Jukebox;

[HarmonyPatch]
public static class CustomJukeboxTrackPatches
{
    internal static readonly SelfCheckingDictionary<BZJukebox.UnlockableTrack, CustomJukeboxTrack> customTracks = new("customTracks");

    internal static GameObject defaultDiskPrefab;

    [HarmonyPatch(typeof(BZJukebox), nameof(BZJukebox.Awake))]
    public static class AwakeWorkaround
    {
        // base Awake assumes all unlockable tracks are events
        // so we need to add ours afterwards (and remove them on game unload)
        [HarmonyPrefix, UsedImplicitly]
        public static void ClearCustomTracks()
        {
            foreach (BZJukebox.UnlockableTrack trackId in customTracks.Keys)
                BZJukebox.unlockableMusic.Remove(trackId);
        }

        [HarmonyPostfix, UsedImplicitly]
        public static void AddCustomTracks(BZJukebox __instance)
        {
            foreach (KeyValuePair<BZJukebox.UnlockableTrack, CustomJukeboxTrack> pair in customTracks)
            {
                CustomJukeboxTrack track = pair.Value;

                BZJukebox.unlockableMusic[pair.Key] = track.identifier;
                BZJukebox.musicLabels[track.identifier] = track.trackLabel; // only read inside Awake but why not
                __instance._info[track.identifier] = track;
            }

            if (!defaultDiskPrefab) CoroutineHost.StartCoroutine(GetJukeboxDiskPrefab());
        }
    }

    private static IEnumerator GetJukeboxDiskPrefab()
    {
        // const string diskClassId = "5108080f-242b-49e8-9b91-d01d6bbe138c";
        const string diskPrefabPath = "Misc/JukeboxDisk8.prefab";
        IPrefabRequest request = PrefabDatabase.GetPrefabForFilenameAsync(diskPrefabPath);
        yield return request;

        if (!request.TryGetPrefab(out defaultDiskPrefab))
            throw new Exception("Could not get prefab for jukebox disk!");
    }

    [HarmonyPatch(typeof(IntroVignette), nameof(IntroVignette.OnDone))]
    [HarmonyPostfix]
    public static void SetupUnlocksForCustomTracks()
    {
        // duplicate disks are not a problem - they self-destruct on Start if already unlocked
        customTracks.ForEach(pair => pair.Value.SetupUnlock(pair.Key));
    }

    [HarmonyPatch(typeof(BZJukebox), nameof(BZJukebox.ScanInternal))]
    [HarmonyPostfix]
    public static void RestoreCustomTrackInfoAfterScan(BZJukebox __instance)
    {
        // base ScanInternal cleans _info of all non-event TrackInfo objects

        foreach (CustomJukeboxTrack track in customTracks.Values)
        {
            __instance._info[track.identifier] = track;
        }
    }

    [HarmonyPatch(typeof(BZJukebox), nameof(BZJukebox.UpdateLowLevel))]
    [HarmonyPrefix]
    public static bool AllowPlayingCustomTracks(BZJukebox __instance)
    {
        if (!__instance.IsTrackCustom(out CustomJukeboxTrack track)) return true;

        if (track.IsSoundValid())
            return !track.isStream || NetStreamCompatPatches.UpdateStream(__instance);

        InitCustomTrack(__instance, track);
        return false;
    }

    private static void InitCustomTrack(BZJukebox jukebox, CustomJukeboxTrack track)
    {
        if (!track.IsSoundValid())
        {
            if (track.IsRemote)
            {
                RESULT result = RuntimeManager.CoreSystem.createSound(track.url, MODE._3D | MODE.CREATESTREAM | MODE.NONBLOCKING | MODE._3D_LINEARSQUAREROLLOFF, ref jukebox._exinfo, out track.sound);
                BZJukebox.ERRCHECK(result);
            }
            else
            {
                track.sound = AudioUtils.CreateSound(track.audioClip, AudioUtils.StandardSoundModes_3D);
            }
        }
        jukebox._sound = track.sound;
    }

    [HarmonyPatch(typeof(BZJukebox), nameof(BZJukebox.HandleOpenError))]
    [HarmonyPrefix]
    public static void RemoveErroringTrack(BZJukebox __instance)
    {
        if (!__instance.IsTrackCustom(out CustomJukeboxTrack track)) return;

        LOGGER.LogError($"Could not load track '{track.identifier}' from {track.url}, removing track from playlist");
        __instance._playlist.Remove(track.identifier);
        BZJukebox.UnlockableTrack trackId = track;
        BZJukebox.unlockableMusic.Remove(trackId);
        Player.main.unlockedTracks.Remove(trackId);
        // customTracks.Remove(trackId);
    }

    [HarmonyPatch(typeof(BZJukebox), nameof(BZJukebox.UpdateInfo))]
    [HarmonyPrefix]
    public static bool DisplayInfoForCustomTracks(BZJukebox __instance)
    {
        if (!__instance.IsTrackCustom(out CustomJukeboxTrack track)) return true;

        __instance._sound.getOpenState(out OPENSTATE state, out _, out _, out _);
        bool hasInfo = __instance._info.TryGetValue(track.identifier, out BZJukebox.TrackInfo info);

        // streams can have their info change during playback
        bool assignOnce = track.IsLocal || track.overrideTrackLabel;
        if (hasInfo && assignOnce) return true;

        BZJukebox.TrackInfo newInfo = (BZJukebox.TrackInfo) track;
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
            newInfo.label = new CustomJukeboxTrack.TrackLabel { artist = artist, title = title };

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
    public static bool IsTrackCustom(this BZJukebox jukebox, out CustomJukeboxTrack track)
        => CustomJukeboxTrack.TryGetCustomTrack(jukebox._file, out track);

    public static bool IsTrackCustom(this JukeboxInstance jukebox, out CustomJukeboxTrack track)
        => CustomJukeboxTrack.TryGetCustomTrack(jukebox._file, out track);

    public static bool IsPlayingStream(this BZJukebox jukebox)
        => IsTrackCustom(jukebox, out CustomJukeboxTrack track) && track.isStream;
    public static bool IsPlayingStream(this JukeboxInstance jukebox)
        => IsTrackCustom(jukebox, out CustomJukeboxTrack track) && track.isStream;
}
