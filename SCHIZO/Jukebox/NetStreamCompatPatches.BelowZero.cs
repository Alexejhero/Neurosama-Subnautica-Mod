using FMOD;
using HarmonyLib;
using BZJukebox = Jukebox;

namespace SCHIZO.Jukebox;

[HarmonyPatch]
public static class NetStreamCompatPatches
{
    // netstreams don't support seeking and will commit ERR_INVALID_HANDLE if anything tries to seek them
    // this check *might* be a bit heavy to do on every single call but... meh
    [HarmonyPatch(typeof(Channel), nameof(Channel.setPosition))]
    [HarmonyPrefix]
    public static bool DontSeekNetStreams(Channel __instance)
    {
        if (!BZJukebox._main || __instance.handle != BZJukebox._main._channel.handle) return true;

        return !BZJukebox._main.IsPlayingStream(out _);
    }

    internal static bool UpdateStream(BZJukebox jukebox, CustomJukeboxTrack track)
    {
        // prevents the sound/channel from turning "virtual"
        // when sounds become inaudible, they turn "virtual" and get paused;
        // when they become audible again, FMOD seeks them forward - and we can't seek streams
        jukebox._channel.setPriority(0);
        jukebox._length = 0; // hide it, it's meaningless on streams

        // prevent console spam on error
        bool canContinue = track.IsSoundValid(out OPENSTATE state) && state != OPENSTATE.ERROR;
        if (!canContinue)
            jukebox.StopInternal();
        return canContinue;
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

        if (!hasInfo || info.label != __instance.textFile.text)
        {
            // LOGGER.LogWarning($"Updating label because {(!hasInfo ? "no info" : $"{info.label} != {__instance.textFile.text}")}");
            __instance.SetLabel(isPlaying && hasInfo ? info.label : track.trackLabel);
        }
    }
}
