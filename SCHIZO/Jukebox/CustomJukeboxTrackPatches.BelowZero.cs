using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using HarmonyLib;
using Nautilus.Utility;
using SCHIZO.Helpers;
using TMPro;
using UnityEngine;
using UWE;
using BZJukebox = Jukebox;

namespace SCHIZO.Jukebox;

[HarmonyPatch]
public static class CustomJukeboxTrackPatches
{
    internal static readonly SelfCheckingDictionary<BZJukebox.UnlockableTrack, CustomJukeboxTrack> customTracks = new("customTracks");
    internal static bool AwakePatchFailed { get; private set; } = false;

    static CustomJukeboxTrackPatches()
    {
        CoroutineHost.StartCoroutine(InitJukebox());
        SaveUtils.RegisterOnQuitEvent(() => CoroutineHost.StartCoroutine(InitJukebox()));
    }

    private static IEnumerator InitJukebox()
    {
        while (PlatformUtils.main.GetServices() == null)
            yield return null;

        _ = BZJukebox.main;
    }

    [HarmonyPatch(typeof(BZJukebox), nameof(BZJukebox.Awake))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> AwakeWorkaround(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        // base Awake assumes all unlockable tracks are events
        // but tracks sourced from netstreams/assets are not
        // ergo, an oopsie woopsie happens
        // this transpiler adds a check to skip tracks that aren't FMOD events

        CodeMatcher matcher = new(instructions, generator);
        MethodBody body = original.GetMethodBody();

        // find the loop init
        /// for (int i = 0; i < list.Count; i++)
        ///      ^^^^^^^^^
        matcher.MatchForward(false,
            new CodeMatch(OpCodes.Ldc_I4_0),
            new CodeMatch(ci => ci.StoresLocal(type: typeof(int), method: body)),
            new CodeMatch(OpCodes.Br)
        );
        if (!matcher.IsValid) goto bad;
        matcher.Advance(1);
        LocalBuilder loopIndex = (LocalBuilder) matcher.Operand;
        matcher.Advance(1);
        Label loopCondition = (Label) matcher.Operand;

        // look for where to insert our instructions
        matcher.MatchForward(false,
            /// string text = list[i];
            new CodeMatch(ci => ci.StoresLocal(type: typeof(string), method: body)),
            /// Jukebox.ERRCHECK(RuntimeManager.GetEventDescription(text).getLength(out len));
            ///  match this call ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
            new CodeMatch(ci => ci.LoadsLocal(type: typeof(string), method: body)),
            new CodeMatch(ci => ci.Calls(AccessTools.Method(typeof(RuntimeManager), nameof(RuntimeManager.GetEventDescription), [typeof(string)]))),
            new CodeMatch(ci => ci.StoresLocal(type: typeof(EventDescription), method: body))
        );
        if (!matcher.IsValid) goto bad;
        LocalBuilder textLocal = (LocalBuilder) matcher.Operand;

        matcher.Advance(1); // insert before loading the "text" local for the RuntimeManager.GetEventDescription call
        matcher.InsertAndAdvance(
            /// if (!Jukebox.IsEvent(text)) continue;
            new CodeInstruction(OpCodes.Ldloc, textLocal),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BZJukebox), nameof(BZJukebox.IsEvent))),
            new CodeInstruction(OpCodes.Brfalse)
        );
        CodeInstruction branch = matcher.InstructionAt(-1);
        // we have a label pointing to the loop condition
        // but "continue" needs to branch to the increment
        // init -> body -> increment -> condition -v
        //          ^-------------------------------
        matcher.MatchForward(false,
            new CodeMatch(OpCodes.Ldloc_S, loopIndex),
            new CodeMatch(OpCodes.Ldc_I4_1),
            new CodeMatch(OpCodes.Add),
            new CodeMatch(OpCodes.Stloc_S, loopIndex),
            new CodeMatch(ci => ci.labels.Contains(loopCondition))
        );
        if (!matcher.IsValid) goto bad;

        matcher.CreateLabel(out Label loopIncrement);
        branch.operand = loopIncrement;
        LOGGER.LogDebug("Patched Jukebox.Awake to support non-FMOD tracks");
        return matcher.InstructionEnumeration();

        bad:
        LOGGER.LogError("Could not patch Jukebox.Awake to support non-FMOD tracks!\n" +
            "Those tracks will not be registered to avoid breaking the whole jukebox.");
        AwakePatchFailed = true;
        return instructions;
    }

    [HarmonyPatch(typeof(JukeboxInstance), nameof(JukeboxInstance.Start))]
    [HarmonyPatch(typeof(uGUI_JukeboxLabel), nameof(uGUI_JukeboxLabel.Show))]
    [HarmonyPostfix]
    public static void EnableRichText(object __instance)
    {
        TMP_Text text = __instance switch
        {
            JukeboxInstance jukebox => jukebox.textFile,
            uGUI_JukeboxLabel seatruckLabel => seatruckLabel.textFile,
            _ => null,
        };
        if (text) text.richText = true;
    }

    [HarmonyPatch(typeof(IntroVignette), nameof(IntroVignette.OnDone))]
    [HarmonyPostfix]
    public static void SetupUnlocksForCustomTracks()
    {
        customTracks.ForEach(pair => pair.Value.SetupUnlock());
    }

    [HarmonyPatch(typeof(BZJukebox), nameof(BZJukebox.ScanInternal))]
    [HarmonyPatch(typeof(BZJukebox), nameof(BZJukebox.ReleaseInstance))]
    [HarmonyPostfix]
    public static void RestoreCustomTrackInfoAfterScan(BZJukebox __instance)
    {
        // base ScanInternal cleans _info of all non-event TrackInfo objects
        // we also want to clean up labels from stream metadata on "instance release" (= on stop)

        foreach (CustomJukeboxTrack track in customTracks.Values)
        {
            __instance._info[track.JukeboxIdentifier] = track.ToTrackInfo();
        }
    }

    [HarmonyPatch(typeof(BZJukebox), nameof(BZJukebox.UpdateLowLevel))]
    [HarmonyPrefix]
    public static bool AllowPlayingCustomTracks(BZJukebox __instance)
    {
        if (!__instance.IsTrackCustom(out CustomJukeboxTrack track)) return true;

        if (track.IsSoundValid(out OPENSTATE state))
        {
            if (state == OPENSTATE.PLAYING) track.OnPlay();
            return !track.isStream || NetStreamCompatPatches.UpdateStream(__instance, track);
        }
        if ((int)state != -1)
        {
            LOGGER.LogWarning($"Track has handle but is in state {state}, this is unusual");
            __instance.HandleOpenError();
        }
        else
        {
            InitCustomTrack(__instance, track);
        }
        return false;
    }

    private static void InitCustomTrack(BZJukebox jukebox, CustomJukeboxTrack track)
    {
        track.sound.release();
        track.sound.clearHandle();
        if (track.IsRemote)
        {
            RESULT result = RuntimeManager.CoreSystem.createSound(track.url, MODE._3D | MODE.CREATESTREAM | MODE.NONBLOCKING | MODE._3D_LINEARSQUAREROLLOFF, ref jukebox._exinfo, out track.sound);
            BZJukebox.ERRCHECK(result);
        }
        else
        {
            track.sound = AudioUtils.CreateSound(track.audioClip, AudioUtils.StandardSoundModes_3D | MODE.NONBLOCKING);
        }
        jukebox._sound = track.sound;
    }

    [HarmonyPatch(typeof(BZJukebox), nameof(BZJukebox.HandleOpenError))]
    [HarmonyPrefix]
    public static bool RemoveErroringTrack(BZJukebox __instance)
    {
        if (!__instance.IsTrackCustom(out CustomJukeboxTrack track)) return true;

        track.OnLoadFail();
        if (track.ShouldRetryLoad)
        {
            LOGGER.LogWarning("Custom track failed to load, trying again");
            InitCustomTrack(__instance, track);
            return false;
        }
        LOGGER.LogError($"Could not load track '{track.identifier}'{(track.IsRemote ? $" from {track.url}" : "")}, removing from playlist");
        __instance._playlist.Remove(track.identifier);
        BZJukebox.unlockableMusic.Remove(track);

        return true;
    }

    [HarmonyPatch(typeof(BZJukebox), nameof(BZJukebox.UpdateInfo))]
    [HarmonyPrefix]
    public static bool DisplayInfoForCustomTracks(BZJukebox __instance)
    {
        if (!__instance.IsTrackCustom(out CustomJukeboxTrack track)) return true;

        bool hasInfo = __instance._info.TryGetValue(track.identifier, out BZJukebox.TrackInfo info);

        // streams can have their info change during playback
        bool assignOnce = !track.IsRemote || track.overrideTrackLabel;
        if (hasInfo && assignOnce) return true;

        BZJukebox.TrackInfo newInfo = track.ToTrackInfo(true);
        if (assignOnce)
        {
            //LOGGER.LogWarning($"Set info from asset/remote - ({newInfo.label},{newInfo.length})");
            __instance.SetInfo(track.identifier, newInfo);
            return false;
        }
        if (!track.IsSoundValid(out OPENSTATE state))
        {
            LOGGER.LogWarning($"Stream suddenly became invalid!\nOPENSTATE: {state};\nhas handle: {track.sound.hasHandle()};\nRESULT: {track.sound.getMode(out _)}");
            __instance.StopInternal();
            return false;
        }

        if (state is OPENSTATE.PLAYING)
        {
            __instance.TryGetArtistAndTitle(__instance._sound, out string artist, out string title);
            if (!track.isStream) __instance._sound.getLength(out newInfo.length, TIMEUNIT.MS);

            string newLabel = CustomJukeboxTrack.GetTrackLabel(artist, title);
            newInfo.label = string.IsNullOrEmpty(newLabel)
                ? track.trackLabel
                : track.FormatTrackLabel(newLabel, true);

            if (info.label != newInfo.label)
            {
                //LOGGER.LogWarning($"Set info from stream - ({newInfo.label},{newInfo.length})");
                __instance.SetInfo(track.identifier, newInfo);
            }
        }
        return false;
    }

    [HarmonyPatch(typeof(BZJukebox), nameof(BZJukebox.HandleLooping))]
    [HarmonyPostfix]
    public static void DontAutoSwitchToStreams(BZJukebox __instance)
    {
        if (__instance._repeat != BZJukebox.Repeat.All) return;

        // Continue looping if the current track is a stream
        if (__instance.IsPlayingStream(out _))
        {
            __instance.HandleLooping();
        }
    }

    [HarmonyPatch(typeof(JukeboxInstance), nameof(JukeboxInstance.file), MethodType.Setter)]
    [HarmonyPostfix]
    public static void OnSwitchingTracks(JukeboxInstance __instance)
    {
        // can't seek streams
        __instance.SetPositionKnobVisible(!__instance.IsPlayingStream(out _));
    }

    [HarmonyPatch(typeof(File), nameof(File.Exists))]
    [HarmonyPostfix]
    public static void DebugStuff(string path)
    {
        if (string.IsNullOrEmpty(path)) return;

        BZJukebox jukebox = BZJukebox._main;
        if (!jukebox || !jukebox.IsTrackCustom(out CustomJukeboxTrack track)) return;
        if (!path.EndsWith(track.identifier)) return;

        bool handle = track.sound.hasHandle();
        OPENSTATE state = (OPENSTATE) (-1);
        RESULT res = handle
            ? track.sound.getOpenState(out state, out _, out _, out _)
            : RESULT.ERR_INVALID_HANDLE;

        LOGGER.LogError($"""
            dinkDonk Developer! You forgor a {track.identifier}!
            {StackTraceUtility.ExtractStackTrace()}
            handle: {handle}, state: {state} ({res})
            """);
    }
}

public static class JukeboxExtensions
{
    public static bool IsTrackCustom(this BZJukebox jukebox, out CustomJukeboxTrack track)
        => CustomJukeboxTrack.TryGetCustomTrack(jukebox._file, out track) && track.source != CustomJukeboxTrack.Source.FMODEvent;
    public static bool IsTrackCustom(this JukeboxInstance jukebox, out CustomJukeboxTrack track)
        => CustomJukeboxTrack.TryGetCustomTrack(jukebox._file, out track) && track.source != CustomJukeboxTrack.Source.FMODEvent;

    public static bool IsPlayingStream(this BZJukebox jukebox, out CustomJukeboxTrack track)
        => IsTrackCustom(jukebox, out track) && track.IsRemote && track.isStream;
    public static bool IsPlayingStream(this JukeboxInstance jukebox, out CustomJukeboxTrack track)
        => IsTrackCustom(jukebox, out track) && track.IsRemote && track.isStream;
}
