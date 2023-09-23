using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Sounds
{
    [CreateAssetMenu(menuName = "SCHIZO/Sounds/Combined Sound Collection")]
    public sealed partial class CombinedSoundCollection : ScriptableObject
    {
        [SerializeField, ReorderableList] private List<SoundCollection> combineWith;
    }
}
