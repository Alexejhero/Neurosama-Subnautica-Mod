using TriInspector;
using SCHIZO.Registering;
using JetBrains.Annotations;
using SCHIZO.Attributes;
using UnityEngine;

namespace SCHIZO.Jukebox
{
    [CreateAssetMenu(menuName = "SCHIZO/Jukebox/Custom Jukebox Track")]
    [DeclareBoxGroup("track", Title = "Track Info")]
    [DeclareBoxGroup("unlock", Title = "Unlock")]
    public sealed partial class CustomJukeboxTrack : ModRegistryItem
    {
        private enum Source
        {
            Asset,
            Internet
        }

        [Careful, Required]
        public string identifier;

        [SerializeField]
        private Source source;

        [HideIf(nameof(IsRemote))]
        public AudioClip audioClip;

        [ShowIf(nameof(IsRemote)), ValidateInput(nameof(Validate_urlIsHttp)), LabelText("URL")]
        public string url;

        [ShowIf(nameof(IsRemote))]
        [Tooltip("Whether to handle the audio like an endless stream, e.g. internet radio.\nIn-game, this will hide duration and disable seeking.")]
        public bool isStream;

        [GroupNext("track")]

        [ShowIf(nameof(isStream))]
        [Multiline, InfoBox("Use {0} as a placeholder for the track label.\nTextMeshPro rich text tags are supported.")]
        public string streamLabelFormat;

        [ShowIf(nameof(IsRemote))]
        [Tooltip("If not overridden, the remote file or stream's metadata (if any) will be used.")]
        public bool overrideTrackLabel;

        public string trackLabel;

        [GroupNext("unlock")]

        public bool unlockedOnStart = true;

        [HideIf(nameof(unlockedOnStart)), UsedImplicitly]
        [Tooltip("If not set, the disk will use the base game model.")]
        public GameObject diskPrefab;

        [HideIf(nameof(unlockedOnStart)), UsedImplicitly]
        public Vector3 diskSpawnLocation;

        [HideIf(nameof(unlockedOnStart)), UsedImplicitly]
        public AudioClip unlockSound;

        public bool IsLocal => source == Source.Asset;
        public bool IsRemote => source == Source.Internet;

        private TriValidationResult Validate_urlIsHttp()
        {
            if (string.IsNullOrEmpty(url)) return TriValidationResult.Error("URL cannot be empty");
            if (!url.StartsWith("http://")) return TriValidationResult.Error("URL must be HTTP (HTTPS will not work)");
            return TriValidationResult.Valid;
        }
    }
}
