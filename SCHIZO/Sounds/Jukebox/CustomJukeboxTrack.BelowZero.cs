using System;
using FMOD;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;

// ReSharper disable once CheckNamespace
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

    protected override void Register()
    {
        LOGGER.LogDebug($"Registering custom jukebox track {identifier}");

        if (EnumHandler.TryGetValue(identifier, out Jukebox.UnlockableTrack trackId))
        {
            LOGGER.LogWarning($"Someone else has already registered unlockable track {identifier}! ({trackId} at {(int) trackId})");
            return;
        }
        else
        {
            if (EnumHandler.TryAddEntry(identifier, out EnumBuilder<Jukebox.UnlockableTrack> registered))
                trackId = registered.Value;
            else
            {
                LOGGER.LogError($"Could not add Jukebox.UnlockableTrack entry for {identifier}");
                return;
            }
        }
        CustomJukeboxTrackPatches.customTracks[trackId] = this;

        if (!Player.main) return;

        // if we get here, we're registering during a game
        // less than ideal but let's roll with it
        LOGGER.LogWarning($"Setting up unlock for track '{identifier}' during a game! This might not behave how you expect it to.");
        SetupUnlock();
    }

    public static void Register(CustomJukeboxTrack track) => track.Register();

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
        }
        else
        {
            SpawnDisk(trackId, diskSpawnLocation);
        }
    }

    internal GameObject SpawnDisk(Jukebox.UnlockableTrack trackId, Vector3 position)
    {
        bool isDefault = !diskPrefab;
        GameObject prefab = !isDefault ? diskPrefab.gameObject : CustomJukeboxTrackPatches.defaultDiskPrefab;

        GameObject disk = Instantiate(prefab);
        disk.transform.position = position;

        if (isDefault) Destroy(disk.GetComponent<JukeboxDisk>());

        CustomJukeboxDisk diskComp = disk.EnsureComponent<CustomJukeboxDisk>();
        diskComp.track = trackId;
        diskComp.unlockSound = unlockSound;

        disk.GetComponent<LargeWorldEntity>().enabled = false; // don't save

        if (!isDefault) MaterialUtils.ApplySNShaders(disk, 1);

        return disk;
    }
}
