using System;
using FMOD;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;
using BZJukebox = Jukebox;

namespace SCHIZO.Sounds.Jukebox;

public sealed partial class CustomJukeboxTrack
{
    public partial struct TrackLabel
    {
        public override string ToString() => string.IsNullOrEmpty(artist) ? title : $"{artist} - {title}";
        public static implicit operator string(TrackLabel trackLabel) => trackLabel.ToString();
    }

    public uint Length => audioClip ? (uint) audioClip.length * 1000 : 0;

    internal Sound sound;

    public bool IsSoundValid() => sound.hasHandle() && sound.getMode(out _) != RESULT.ERR_INVALID_HANDLE;

    public static implicit operator BZJukebox.TrackInfo(CustomJukeboxTrack track)
    {
        string label = track.trackLabel;
        if (label.Length == 0) label = track.identifier;
        return new BZJukebox.TrackInfo { label = label, length = track.Length };
    }

    public static implicit operator BZJukebox.UnlockableTrack(CustomJukeboxTrack track)
        => EnumHandler.TryGetValue(track.identifier, out BZJukebox.UnlockableTrack id) ? id
            : throw new ArgumentException("Track is not registered, cannot convert to Jukebox.UnlockableTrack", nameof(track));

    protected override void Register()
    {
        LOGGER.LogDebug($"Registering custom jukebox track {identifier}");

        if (EnumHandler.TryGetValue(identifier, out BZJukebox.UnlockableTrack trackId))
        {
            LOGGER.LogWarning($"Someone else has already registered unlockable track {identifier}! ({trackId} at {(int) trackId})");
            return;
        }

        if (EnumHandler.TryAddEntry(identifier, out EnumBuilder<BZJukebox.UnlockableTrack> registered))
        {
            trackId = registered.Value;
        }
        else
        {
            LOGGER.LogError($"Could not add Jukebox.UnlockableTrack entry for {identifier}");
            return;
        }
        CustomJukeboxTrackPatches.customTracks[trackId] = this;

        if (!Player.main) return;

        // if we get here, we're registering during a game
        // less than ideal but let's roll with it
        LOGGER.LogWarning($"Setting up unlock for track '{identifier}' during a game! This might not behave how you expect it to.");
        SetupUnlock();
    }

    public static bool TryGetCustomTrack(string identifier, out CustomJukeboxTrack track)
    {
        track = null;

        return identifier is not null
            && EnumHandler.TryGetValue(identifier, out BZJukebox.UnlockableTrack trackId)
            && TryGetCustomTrack(trackId, out track);
    }

    public static bool TryGetCustomTrack(BZJukebox.UnlockableTrack trackId, out CustomJukeboxTrack track)
        => CustomJukeboxTrackPatches.customTracks.TryGetValue(trackId, out track);

    internal void SetupUnlock(BZJukebox.UnlockableTrack trackId = BZJukebox.UnlockableTrack.None)
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
            BZJukebox.Unlock(trackId, false);
            BZJukebox.main.SetInfo(identifier, this);
        }
        else
        {
            SpawnDisk(trackId, diskSpawnLocation);
        }
    }

    private GameObject SpawnDisk(BZJukebox.UnlockableTrack trackId, Vector3 position)
    {
        GameObject disk = Instantiate(CustomJukeboxTrackPatches.defaultDiskPrefab);
        disk.transform.position = position;

        if (diskPrefab)
        {
            Renderer[] renderers = disk.GetComponentsInChildren<Renderer>();
            renderers.ForEach(r => r.gameObject.SetActive(false));

            GameObject customModel = Instantiate(diskPrefab, disk.transform, false);
            customModel.transform.localPosition = Vector3.zero;

            MaterialUtils.ApplySNShaders(disk, 1);
        }

        Destroy(disk.GetComponent<JukeboxDisk>());

        CustomJukeboxDisk diskComp = disk.EnsureComponent<CustomJukeboxDisk>();
        diskComp.track = trackId;
        diskComp.unlockSound = unlockSound;

        disk.GetComponent<LargeWorldEntity>().enabled = false; // don't save

        return disk;
    }
}
