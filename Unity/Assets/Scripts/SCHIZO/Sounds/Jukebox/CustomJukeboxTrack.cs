using NaughtyAttributes;
using System;
using UnityEngine;

namespace SCHIZO.Sounds.JukeboxButTheNamespaceConflictsWithTheGlobalJukeboxClassName.ThisIsWhyUnityIsTheBestGameEngine
{
    [CreateAssetMenu(menuName = "SCHIZO/Sounds/Jukebox Track")]
    public sealed partial class CustomJukeboxTrack : ScriptableObject
    {
        public enum Source
        {
            Asset,
            Internet
        }
        [ValidateInput(nameof(Validate_identifier))]
        public string identifier;
        public Source source;
        [HideIf(nameof(IsRemote))]
        public AudioClip audioClip;
        [ShowIf(nameof(IsRemote)), ValidateInput(nameof(Validate_urlIsHttp), "Must be an HTTP address. HTTPS is not supported!")]
        public string URL;
        [Tooltip("Whether to handle the audio like an endless stream, e.g. internet radio.\nIn-game, this will hide duration and disable seeking.")]
        [ShowIf(nameof(IsRemote))]
        public bool isStream;

        [InfoBox("If not overridden, the remote file or stream's metadata (if any) will be used.")]
        [BoxGroup("Track Info"), ShowIf(nameof(IsRemote))]
        public bool overrideTrackLabel;
        [BoxGroup("Track Info")]
        public TrackLabel trackLabel;

        private bool Validate_identifier() => !string.IsNullOrEmpty(identifier);
        private bool Validate_urlIsHttp() => URL.StartsWith("http://");
        private bool source_isRemote() => source == Source.Internet;
        public uint Length => audioClip ? (uint) audioClip.length : 0;

        public bool IsLocal => source == Source.Asset;
        public bool IsRemote => source == Source.Internet;
    }

    [Serializable]
    public partial class TrackLabel
    {
        public string artist;
        public string title;

        public override string ToString() => string.IsNullOrEmpty(artist) ? title : $"{artist} - {title}";

        public static implicit operator string(TrackLabel trackLabel) => trackLabel.ToString();
    }
}
