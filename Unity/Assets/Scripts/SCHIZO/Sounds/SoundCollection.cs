using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.API.Unity.Sounds
{
    [CreateAssetMenu(menuName = "SCHIZO/Sounds/Sound Collection")]
    public sealed class SoundCollection : BaseSoundCollection
    {
        [ReorderableList] public List<AudioClip> sounds;

        public override IEnumerable<AudioClip> GetSounds() => sounds;
    }
}
