using NaughtyAttributes;
using SCHIZO.Interop.Subnautica;
using SCHIZO.Sounds.Collections;
using UnityEngine;

namespace SCHIZO.Creatures.Components
{
    public partial class GetCarried : CustomCreatureAction
    {
        public bool likesBeingCarried;
        public Transform pickupPoint;
        [BoxGroup("Sounds")]
        public _FMOD_CustomEmitter emitter;
        [BoxGroup("Sounds")]
        public float carryNoiseInterval = 5f;
        [BoxGroup("Sounds")]
        public SoundCollectionInstance pickupSounds;
        [BoxGroup("Sounds")]
        public SoundCollectionInstance carrySounds;
        [BoxGroup("Sounds")]
        public SoundCollectionInstance releaseSounds;
    }
}
