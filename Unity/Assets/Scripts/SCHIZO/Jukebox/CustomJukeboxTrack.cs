using NaughtyAttributes;
using SCHIZO.Attributes.Visual;
using SCHIZO.Registering;
using JetBrains.Annotations;
using UnityEngine;

namespace SCHIZO.Jukebox
{
    [CreateAssetMenu(menuName = "SCHIZO/Jukebox/Custom Jukebox Track")]
    public sealed partial class CustomJukeboxTrack : ModRegistryItem
    {
        private enum Source
        {
            Asset,
            Internet
        }

        [Careful, Required_string]
        public string identifier;

        [SerializeField]
        private Source source;

        [HideIf(nameof(IsRemote))]
        public AudioClip audioClip;

        [ShowIf(nameof(IsRemote)), ValidateInput(nameof(Validate_urlIsHttp), "Must be an HTTP address. HTTPS does not work!"), Label("URL")]
        public string url;

        [ShowIf(nameof(IsRemote))]
        [Tooltip("Whether to handle the audio like an endless stream, e.g. internet radio.\nIn-game, this will hide duration and disable seeking.")]
        public bool isStream;

        [BoxGroup("Track Info"), ShowIf(nameof(isStream))]
        [Multiline, InfoBox("Use {0} as a placeholder for the track label.\nTextMeshPro rich text tags are supported.")]
        public string streamLabelFormat;

        [BoxGroup("Track Info"), ShowIf(nameof(IsRemote))]
        [Tooltip("If not overridden, the remote file or stream's metadata (if any) will be used.")]
        public bool overrideTrackLabel;

        [BoxGroup("Track Info")]
        public string trackLabel;

        [BoxGroup("Unlock")]
        public bool unlockedOnStart = true;

        [BoxGroup("Unlock"), HideIf(nameof(unlockedOnStart)), UsedImplicitly]
        [Tooltip("If not set, the disk will use the base game model.")]
        public GameObject diskPrefab;

        [BoxGroup("Unlock"), HideIf(nameof(unlockedOnStart)), UsedImplicitly]
        public Vector3 diskSpawnLocation;

        [BoxGroup("Unlock"), HideIf(nameof(unlockedOnStart)), UsedImplicitly]
        public AudioClip unlockSound;

        public bool IsLocal => source == Source.Asset;
        public bool IsRemote => source == Source.Internet;

        private bool Validate_urlIsHttp() => url.StartsWith("http://");
    }
}
