using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Sounds
{
    [CreateAssetMenu(menuName = "SCHIZO/Sounds/Combined Sound Collection")]
    public sealed class CombinedSoundCollection : BaseSoundCollection
    {
        [SerializeField, ReorderableList] private List<SoundCollection> combineWith;

        public override IEnumerable<AudioClip> GetSounds() => combineWith.SelectMany(s => s.GetSounds());
    }
}
