using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Sounds
{
    [CreateAssetMenu(menuName = "SCHIZO/Sounds/Sound Collection")]
    public sealed class SoundCollection : BaseSoundCollection
    {
        [SerializeField, ReorderableList] private List<AudioClip> sounds;

        public override IEnumerable<AudioClip> GetSounds() => sounds;
    }
}
