using NaughtyAttributes;
using SCHIZO.Attributes;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures
{
    public sealed partial class HurtSoundPlayer : MonoBehaviour
    {
        [Required, SerializeField] private BaseSoundCollection hurtSounds;
        [Required, ExposedType("FMOD_CustomEmitter")] public MonoBehaviour emitter;
    }
}
