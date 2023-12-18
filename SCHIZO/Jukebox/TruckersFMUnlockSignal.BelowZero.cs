using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using Nautilus.Utility;
using Story;
using UnityEngine;
using BZJukebox = Jukebox;
using TrackId = Jukebox.UnlockableTrack;

namespace SCHIZO.Jukebox;

[HarmonyPatch]
partial class TruckersFMUnlockSignal
{
    public static Action<TrackId> onTrackUnlocked;

    private int unlockedTracks;
    private SignalPing signal;
    private string SpriteKey => $"CustomTrackSignal({track.identifier})";
    private PingType PingType => (PingType)SpriteKey.GetHashCode();
    public IEnumerator Start()
    {
        onTrackUnlocked += OnTrackUnlocked;
        SaveUtils.RegisterOnQuitEvent(Reset);
        yield return new WaitUntil(() => SpriteManager.hasInitialized);
        Dictionary<string, Sprite> pingSprites = SpriteManager.atlases[SpriteManager.mapping[SpriteManager.Group.Pings]];
        pingSprites[SpriteKey] = signalSprite;
        PingManager.sCachedPingTypeStrings.valueToString[PingType] = SpriteKey;
        PingManager.sCachedPingTypeTranslationStrings.valueToString[PingType] = "Signal";
    }

    public void OnDestroy()
    {
        onTrackUnlocked -= OnTrackUnlocked;
        SaveUtils.UnregisterOnQuitEvent(Reset);
    }

    public void OnTrackUnlocked(TrackId trackId)
    {
        if (!enabled) return;

        if (trackId == track)
        {
            DestroySignal();
            enabled = false;
        }

        if (signal) return;
        if (customOnly && !CustomJukeboxTrack.IsTrackCustom(trackId)) return;

        unlockedTracks++;
        if (unlockedTracks >= requiredTracks)
            CreateSignal();
    }

    private void CreateSignal()
    {
        signal = Instantiate(StoryGoalManager.main.onGoalUnlockTracker.signalPrefab, transform)
            .GetComponent<SignalPing>();
        signal.descriptionKey = signalName;
        signal.pingInstance.SetType(PingType);
        signal.pingInstance.colorIndex = 1; // red
        signal.pos = track.diskSpawnLocation.position;
    }

    private void DestroySignal()
    {
        if (signal) Destroy(signal.gameObject);
    }

    private void Reset()
    {
        unlockedTracks = 0;
        enabled = true;
        DestroySignal();
    }

    [HarmonyPatch(typeof(BZJukebox), nameof(BZJukebox.OnUnlock))]
    [HarmonyPostfix]
    public static void HookOnUnlock(BZJukebox __instance, TrackId track)
    {
        onTrackUnlocked?.Invoke(track);
    }
    [HarmonyPatch(typeof(BZJukebox), nameof(BZJukebox.Deserialize))]
    [HarmonyPostfix]
    public static void HookOnDeserialize(BZJukebox __instance, List<TrackId> tracks)
    {
        foreach (TrackId track in tracks)
            onTrackUnlocked?.Invoke(track);
    }
}
