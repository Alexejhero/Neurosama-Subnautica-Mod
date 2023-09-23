using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Sounds
{
    [CreateAssetMenu(menuName = "SCHIZO/Sounds/Sound Collection")]
    public sealed partial class SoundCollection : ScriptableObject
    {
        [ReorderableList] public List<AudioClip> sounds;
    }
}
