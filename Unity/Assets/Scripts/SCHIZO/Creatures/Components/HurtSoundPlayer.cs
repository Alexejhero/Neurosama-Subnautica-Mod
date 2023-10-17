using JetBrains.Annotations;
using NaughtyAttributes;
using SCHIZO.Attributes.Typing;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Components
{
    public sealed partial class HurtSoundPlayer : MonoBehaviour
    {
        [Required, SerializeField, UsedImplicitly] private BaseSoundCollection hurtSounds;
        [Required, ExposedType("FMOD_CustomEmitter")] public MonoBehaviour emitter;
    }
}
