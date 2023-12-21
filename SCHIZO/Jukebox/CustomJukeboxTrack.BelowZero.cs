using System;
using FMOD;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.Helpers;
using UnityEngine;
using BZJukebox = Jukebox;

namespace SCHIZO.Jukebox;

public sealed partial class CustomJukeboxTrack
{
    public uint Length => audioClip ? (uint) audioClip.length * 1000 : 0;

    internal Sound sound;
    public int LoadFailCount { get; private set; }
    public void OnLoadFail() => LoadFailCount++;
    public void OnPlay() => LoadFailCount = 0;
    public bool IsRemote => source == Source.Internet;
    public bool ShouldRetryLoad => LoadFailCount < (IsRemote ? 3 : 5);
    private string _jukeboxId;
    public string JukeboxIdentifier => _jukeboxId ??= (source == Source.FMODEvent ? fmodEvent : identifier);

    public bool IsSoundValid(out OPENSTATE state)
    {
        state = (OPENSTATE)(-1);
        RESULT res = sound.hasHandle()
            ? sound.getOpenState(out state, out _, out _, out _)
            : RESULT.ERR_INVALID_HANDLE;
        if (res is not (RESULT.OK or RESULT.ERR_INVALID_HANDLE)) LOGGER.LogError("Received error while checking sound state: " + res);
        return !res.ToString().StartsWith("ERR"); // oh yeah it's big brain time
    }

    public static string GetTrackLabel(string artist, string title)
        => string.IsNullOrEmpty(artist) ? title : $"{artist} - {title}";

    public string FormatTrackLabel(string label, bool isPlaying = false)
        => isStream && isPlaying && !string.IsNullOrEmpty(streamLabelFormat)
            ? string.Format(streamLabelFormat, label)
            : label;

    public BZJukebox.TrackInfo ToTrackInfo(bool isPlaying = false)
    {
        string label = FormatTrackLabel(trackLabel, isPlaying);
        if (label.Length == 0) label = identifier;
        return new BZJukebox.TrackInfo { label = label, length = Length };
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
        RegisterInJukebox(BZJukebox._main);
        JukeboxDiskPrefab.Register(this);

        if (!Player.main) return;

        // if we get here, we're registering during a game
        // less than ideal but let's roll with it
        LOGGER.LogWarning($"Setting up unlock for track '{identifier}' during a game! This might not behave how you expect it to.");
        SetupUnlock();
    }

    internal void RegisterInJukebox(BZJukebox jukebox)
    {
        BZJukebox.unlockableMusic[this] = JukeboxIdentifier;
        BZJukebox.musicLabels[JukeboxIdentifier] = trackLabel;

        if (jukebox) jukebox._info[JukeboxIdentifier] = ToTrackInfo(false);
    }

    public static bool TryGetCustomTrack(string identifier, out CustomJukeboxTrack track)
    {
        track = null;
        // this will not find event tracks (which is intended)
        return identifier is not null
            && EnumHandler.TryGetValue(identifier, out BZJukebox.UnlockableTrack trackId)
            && TryGetCustomTrack(trackId, out track);
    }

    public static bool TryGetCustomTrack(BZJukebox.UnlockableTrack trackId, out CustomJukeboxTrack track)
        => CustomJukeboxTrackPatches.customTracks.TryGetValue(trackId, out track);

    public static bool IsTrackCustom(BZJukebox.UnlockableTrack trackId)
        => trackId is < BZJukebox.UnlockableTrack.None
                   or > BZJukebox.UnlockableTrack.Track9;

    internal void SetupUnlock()
    {
        if (!Player.main || !GameModeManager.HaveGameOptionsSet)
        {
            LOGGER.LogError($"Can't set up unlock for {identifier} with no {(!Player.main ? "player" : "game options")}!");
            return;
        }

        if (unlockedOnStart || !GameModeManager.GetOption<bool>(GameOption.Story))
        {
            BZJukebox.Unlock(this, false);
            BZJukebox.main.SetInfo(JukeboxIdentifier, ToTrackInfo(false));
        }
    }
}
