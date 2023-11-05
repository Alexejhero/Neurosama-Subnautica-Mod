using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Sounds.Collections
{
    [CreateAssetMenu(menuName = "SCHIZO/Sounds/Combined Sound Collection")]
    public sealed class CombinedSoundCollection : SoundCollection
    {
        [ListDrawerSettings] public List<SoundCollection> combineWith;

        public override IEnumerable<AudioClip> GetSounds() => combineWith.SelectMany(s => s.GetSounds());
        protected override bool ShowAudioClipList => false;
    }
}
