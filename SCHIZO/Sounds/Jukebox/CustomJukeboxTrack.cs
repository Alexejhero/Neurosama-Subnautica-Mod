using FMOD;

namespace SCHIZO.Sounds.JukeboxButTheNamespaceConflictsWithTheGlobalJukeboxClassName.ThisIsWhyUnityIsTheBestGameEngine;

public sealed partial class CustomJukeboxTrack
{
    public static void Register(CustomJukeboxTrack track)
    {
        LOGGER.LogDebug($"Registering custom jukebox track {track.identifier}");
        CustomJukeboxTrackPatches.customTracks.Add(track.identifier, track);
    }

    public static bool TryGetCustomTrack(string identifier, out CustomJukeboxTrack track)
        => CustomJukeboxTrackPatches.customTracks.TryGetValue(identifier ?? "", out track);

    internal Sound sound;

    public bool SoundIsValid => sound.hasHandle() && sound.getMode(out _) != RESULT.ERR_INVALID_HANDLE;

    public static implicit operator Jukebox.TrackInfo(CustomJukeboxTrack track)
    {
        string label = track.trackLabel;
        if (label.Length == 0) label = track.identifier;
        return new() { label = label, length = track.Length };
    }
}
