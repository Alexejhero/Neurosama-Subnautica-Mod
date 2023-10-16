using JetBrains.Annotations;
using NaughtyAttributes;
using SCHIZO.Attributes.Typing;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark
{
    public sealed partial class ErmsharkAttack
    {
        [BoxGroup("Sounds"), SerializeField, Required, UsedImplicitly] private BaseSoundCollection attackSounds;
        [BoxGroup("Sounds"), SerializeField, Required, ExposedType("FMOD_CustomEmitter"), UsedImplicitly] private MonoBehaviour emitter;
    }
}
