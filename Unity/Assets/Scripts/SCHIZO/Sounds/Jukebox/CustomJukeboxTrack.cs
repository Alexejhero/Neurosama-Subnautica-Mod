using NaughtyAttributes;
using SCHIZO.Attributes.Visual;
using SCHIZO.Registering;
using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace SCHIZO.Sounds.Jukebox
{
    [CreateAssetMenu(menuName = "SCHIZO/Sounds/Jukebox Track")]
    public sealed partial class CustomJukeboxTrack : ModRegistryItem
    {
        private enum Source
        {
            Asset,
            Internet
        }

        [Serializable]
        public partial struct TrackLabel
        {
            public string artist;
            public string title;
        }

        [Careful, ValidateInput(nameof(Validate_identifier), "Identifier must not be empty")]
        public string identifier;

        [SerializeField]
        private Source source;

        [HideIf(nameof(IsRemote))]
        public AudioClip audioClip;

        [FormerlySerializedAs("URL"), ShowIf(nameof(IsRemote)), ValidateInput(nameof(Validate_urlIsHttp), "Must be an HTTP address. HTTPS does not work!"), Label("URL")]
        public string url;

        [Tooltip("Whether to handle the audio like an endless stream, e.g. internet radio.\nIn-game, this will hide duration and disable seeking.")]
        [ShowIf(nameof(IsRemote))]
        public bool isStream;

        [InfoBox("If not overridden, the remote file or stream's metadata (if any) will be used.")]
        [BoxGroup("Track Info"), ShowIf(nameof(IsRemote))]
        public bool overrideTrackLabel;

        [BoxGroup("Track Info")]
        public TrackLabel trackLabel;

        [BoxGroup("Unlock")]
        public bool unlockedOnStart = true;

        [BoxGroup("Unlock"), HideIf(nameof(unlockedOnStart)), UsedImplicitly]
        [InfoBox("If not set, the disk will use the base game model.")]
        public CustomJukeboxDisk diskPrefab;

        [BoxGroup("Unlock"), HideIf(nameof(unlockedOnStart)), UsedImplicitly]
        public Vector3 diskSpawnLocation;

        [BoxGroup("Unlock"), HideIf(nameof(unlockedOnStart)), UsedImplicitly]
        public AudioClip unlockSound;

        public bool IsLocal => source == Source.Asset;
        public bool IsRemote => source == Source.Internet;

        private bool Validate_identifier() => !string.IsNullOrEmpty(identifier);
        private bool Validate_urlIsHttp() => url.StartsWith("http://");
    }
}
