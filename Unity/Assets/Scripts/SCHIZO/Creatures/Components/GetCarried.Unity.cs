using NaughtyAttributes;
using SCHIZO.Attributes.Typing;
using UnityEngine;

namespace SCHIZO.Creatures.Components
{
    partial class GetCarried
    {
        [BoxGroup("Sounds"), ValidateType("FMOD_CustomEmitter")]
        public MonoBehaviour emitter;
    }
}
