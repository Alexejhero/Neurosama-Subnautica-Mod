using NaughtyAttributes;
using SCHIZO.Attributes;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark
{
    public sealed partial class ErmsharkAttack
    {
        [BoxGroup("Sounds"), SerializeField, Required] private BaseSoundCollection attackSounds;
        [BoxGroup("Sounds"), SerializeField, Required, ExposedType("FMOD_CustomEmitter")] private MonoBehaviour emitter;
    }
}
