using System;
using FMOD;
using Nautilus.Handlers;
using UnityEngine;

namespace SCHIZO.Sounds.Jukebox_;

public sealed partial class CustomJukeboxTrack
{
    internal Sound sound;

    public bool IsSoundValid() => sound.hasHandle() && sound.getMode(out _) != RESULT.ERR_INVALID_HANDLE;

    public static implicit operator Jukebox.TrackInfo(CustomJukeboxTrack track)
    {
        string label = track.trackLabel;
        if (label.Length == 0) label = track.identifier;
        return new() { label = label, length = track.Length };
    }

    public static implicit operator Jukebox.UnlockableTrack(CustomJukeboxTrack track)
        => EnumHandler.TryGetValue(track.identifier, out Jukebox.UnlockableTrack id) ? id
            : throw new ArgumentException("Track is not registered, cannot convert to Jukebox.UnlockableTrack", nameof(track));

    public static void Register(CustomJukeboxTrack track)
    {
        LOGGER.LogDebug($"Registering custom jukebox track {track.identifier}");

        if (EnumHandler.TryGetValue(track.identifier, out Jukebox.UnlockableTrack trackId))
        {
            LOGGER.LogWarning($"Someone else has already registered unlockable track {track.identifier}! ({trackId} at {(int) trackId})");
            return;
        }
        else
        {
            if (EnumHandler.TryAddEntry(track.identifier, out EnumBuilder<Jukebox.UnlockableTrack> registered))
                trackId = registered.Value;
            else
            {
                LOGGER.LogError($"Could not add Jukebox.UnlockableTrack entry for {track.identifier}");
                return;
            }
        }
        CustomJukeboxTrackPatches.customTracks[trackId] = track;

        if (!Player.main) return;

        // if we get here, we're registering during a game
        // less than ideal but let's roll with it
        LOGGER.LogWarning($"Setting up unlock for track '{track.identifier}' during a game! This might not behave how you expect it to.");
        track.SetupUnlock();
    }

    public static bool TryGetCustomTrack(string identifier, out CustomJukeboxTrack track)
    {
        track = null;

        return identifier is not null
            && EnumHandler.TryGetValue(identifier, out Jukebox.UnlockableTrack trackId)
            && TryGetCustomTrack(trackId, out track);
    }

    public static bool TryGetCustomTrack(Jukebox.UnlockableTrack trackId, out CustomJukeboxTrack track)
        => CustomJukeboxTrackPatches.customTracks.TryGetValue(trackId, out track);

    internal void SetupUnlock(Jukebox.UnlockableTrack trackId = Jukebox.UnlockableTrack.None)
    {
        if (trackId == default)
            trackId = this;

        if (!Player.main || !GameModeManager.HaveGameOptionsSet)
        {
            LOGGER.LogError($"Can't set up unlock for {trackId} with no {(!Player.main ? "player" : "game options")}!");
            return;
        }

        if (unlockedOnStart || !GameModeManager.GetOption<bool>(GameOption.Story))
        {
            Jukebox.Unlock(trackId, false);
            Jukebox.main.SetInfo(identifier, this);
            //LOGGER.LogWarning($"Unlocked {trackLabel} ({identifier})");
        }
        else
        {
            SpawnDisk(trackId, diskSpawnLocation);
            //LOGGER.LogWarning($"Spawned disk for {trackLabel} ({identifier})");
        }
    }

    internal static void SpawnDisk(Jukebox.UnlockableTrack trackId, Vector3 position)
    {
        //LOGGER.LogWarning($"Spawning disk for {trackId} at {position}");
        GameObject disk = Instantiate(CustomJukeboxTrackPatches.jukeboxDiskPrefab);
        disk.transform.position = position;
        disk.GetComponent<JukeboxDisk>().track = trackId;
        disk.GetComponent<LargeWorldEntity>().enabled = false; // don't save
    }
}
