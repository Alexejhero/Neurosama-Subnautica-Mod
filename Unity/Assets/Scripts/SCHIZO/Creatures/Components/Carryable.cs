using FMODUnity;
using SCHIZO.Interop.Subnautica;
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
        [EventRef]
        public string attachSounds;
        [EventRef]
        public string carrySounds;
        [EventRef]
        public string detachSounds;
    }
}
