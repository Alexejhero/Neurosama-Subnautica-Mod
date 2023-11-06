using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Sounds.Collections
{
    [CreateAssetMenu(menuName = "SCHIZO/Sounds/Sound Collection")]
    public partial class SoundCollection : ScriptableObject
    {
        [SerializeField, ListDrawerSettings(AlwaysExpanded = true), ShowIf(nameof(ShowAudioClipList))] private List<AudioClip> sounds;

        public virtual IEnumerable<AudioClip> GetSounds() => sounds;
        protected virtual bool ShowAudioClipList => true;
    }
}
