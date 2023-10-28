using NaughtyAttributes;
using SCHIZO.Attributes.Typing;
using SCHIZO.Sounds;
using UnityEngine;
using UnityEngine.Serialization;

namespace SCHIZO.Creatures.Components
{
    public partial class GetCarried : CustomCreatureAction
    {
        public bool likesBeingCarried;
        public Transform pickupPoint;
        [BoxGroup("Sounds"), ExposedType("FMOD_CustomEmitter")]
        public MonoBehaviour emitter;
        [BoxGroup("Sounds")]
        public float carryNoiseInterval = 5f;
        [BoxGroup("Sounds")]
        public BaseSoundCollection pickupSounds;
        [BoxGroup("Sounds")]
        public BaseSoundCollection carrySounds;
        [BoxGroup("Sounds")]
        public BaseSoundCollection releaseSounds;
    }
}
