using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace SCHIZO.Sounds
{
    [CreateAssetMenu(menuName = "SCHIZO/Sounds/Sound Collection")]
    public partial class SoundCollection : ScriptableObject
    {
        [SerializeField, ReorderableList, ShowIf(nameof(ShowAudioClipList))] private List<AudioClip> sounds;

        public virtual IEnumerable<AudioClip> GetSounds() => sounds;

        protected virtual bool ShowAudioClipList => true;
    }
}
