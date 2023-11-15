using SCHIZO.Interop.Subnautica;
using SCHIZO.Sounds.Collections;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Creatures.Components
{
    [DeclareBoxGroup("Sounds")]
    public partial class Carryable : MonoBehaviour
    {
        public bool likesBeingCarried;
        public Transform attachPlug;

        [GroupNext("Sounds")]
        public _FMOD_CustomEmitter emitter;
        public float carryNoiseInterval = 5f;
        public SoundCollectionInstance attachSounds;
        public SoundCollectionInstance carrySounds;
        public SoundCollectionInstance detachSounds;
    }
}
