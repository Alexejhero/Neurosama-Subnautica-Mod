using SCHIZO.Interop.Subnautica;
using SCHIZO.Sounds.Collections;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Creatures.Components
{
    [DeclareBoxGroup("Sounds")]
    public partial class GetCarried : CustomCreatureAction
    {
        public bool likesBeingCarried;
        public Transform pickupPoint;

        [GroupNext("Sounds")]
        public _FMOD_CustomEmitter emitter;
        public float carryNoiseInterval = 5f;
        public SoundCollectionInstance pickupSounds;
        public SoundCollectionInstance carrySounds;
        public SoundCollectionInstance releaseSounds;
    }
}
