using TriInspector;
using JetBrains.Annotations;
using SCHIZO.Attributes;
using SCHIZO.Registering;
using UnityEngine;
using SCHIZO.Spawns;
using FMODUnity;

namespace SCHIZO.Jukebox
{
    [CreateAssetMenu(menuName = "SCHIZO/Jukebox/Custom Jukebox Track")]
    [DeclareBoxGroup("track", Title = "Track Info")]
    [DeclareBoxGroup("unlock", Title = "Unlock")]
    public sealed partial class CustomJukeboxTrack : ModRegistryItem
    {
        public enum Source
        {
            Asset,
            FMODEvent,
            Internet
        }

        [Careful, Required]
        public string identifier;

        public Source source;

        [ShowIf(nameof(source), Source.Asset)]
        [InfoBox("Audio clips are obsolete, prefer FMOD events for local audio instead", TriMessageType.Warning)]
        public AudioClip audioClip;

        [ShowIf(nameof(source), Source.FMODEvent)]
        [EventRef]
        public string fmodEvent;

        [ShowIf(nameof(source), Source.Internet), ValidateInput(nameof(Validate_urlIsHttp)), LabelText("URL")]
        public string url;

        [ShowIf(nameof(source), Source.Internet)]
        [Tooltip("Whether to handle the audio like an endless stream, e.g. internet radio.\nIn-game, this will hide duration and disable seeking.")]
        public bool isStream;

        [GroupNext("track")]
        [ShowIf(nameof(source), Source.Internet), ShowIf(nameof(isStream))]
        [Multiline, InfoBox("Use {0} as a placeholder for the track label.\nTextMeshPro rich text tags are supported.")]
        public string streamLabelFormat;

        [ShowIf(nameof(source), Source.Internet)]
        [Tooltip("If not overridden, the remote file or stream's metadata (if any) will be used.")]
        public bool overrideTrackLabel;

        public string trackLabel;

        [GroupNext("unlock")]
        public bool unlockedOnStart;

        [HideIf(nameof(unlockedOnStart)), UsedImplicitly]
        [Tooltip("If not set, the disk will use the base game model.")]
        public GameObject diskPrefab;

        [HideIf(nameof(unlockedOnStart)), UsedImplicitly]
        public SpawnLocation diskSpawnLocation;

        [HideIf(nameof(unlockedOnStart)), UsedImplicitly]
        public Sprite unlockSprite;

        [HideIf(nameof(unlockedOnStart)), UsedImplicitly, EventRef]
        public string unlockFmodEvent;

        private TriValidationResult Validate_urlIsHttp()
        {
            if (string.IsNullOrEmpty(url)) return TriValidationResult.Error("URL cannot be empty");
            if (!url.StartsWith("http://")) return TriValidationResult.Error("URL must be HTTP (HTTPS will not work)");
            return TriValidationResult.Valid;
        }
    }
}
