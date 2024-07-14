using Immersion.Formatting;

namespace Immersion.Trackers;

[HarmonyPatch]
public sealed class JukeboxTrackUnlocked : Tracker
{
    internal static readonly HashSet<string> _allowlist = new(StringComparer.InvariantCultureIgnoreCase)
    {
        "truckersfm"
    };

    internal void NotifyUnlocked(Jukebox.UnlockableTrack track)
    {
        if (!Jukebox.unlockableMusic.TryGetValue(track, out string trackId)) return;
        if (!_allowlist.Contains(trackId)) return;

        React(Priority.Low, Format.FormatPlayer($"{{player}} has unlocked a new jukebox track: {Jukebox.GetInfo(trackId).label}"));
    }

    [HarmonyPatch(typeof(Jukebox), nameof(Jukebox.OnUnlock))]
    [HarmonyPostfix]
    private static void OnUnlock(Jukebox.UnlockableTrack track, bool notify)
    {
        if (!notify) return;

        COMPONENT_HOLDER.GetComponent<JukeboxTrackUnlocked>().NotifyUnlocked(track);
    }
}
