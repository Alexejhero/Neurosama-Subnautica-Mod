using TriInspector;
using UnityEngine;

namespace SCHIZO.Jukebox
{
    public sealed partial class TruckersFMUnlockSignal : MonoBehaviour
    {
        [Required]
        public CustomJukeboxTrack track;
        [Tooltip("How many other tracks are required to be unlocked to show this signal")]
        public int requiredTracks;
        [Tooltip("Excludes base game tracks from being counted")]
        public bool customOnly;
        [Required]
        public string signalName;
        public Sprite signalSprite;
    }
}
