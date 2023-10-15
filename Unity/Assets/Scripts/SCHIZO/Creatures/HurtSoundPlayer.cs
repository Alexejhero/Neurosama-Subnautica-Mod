using NaughtyAttributes;
using SCHIZO.Packages.NaughtyAttributes;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures
{
    public sealed partial class HurtSoundPlayer : MonoBehaviour
    {
        [Required, SerializeField] private BaseSoundCollection hurtSounds;
        [Required, ValidateType("FMOD_CustomEmitter")] public MonoBehaviour emitter;
    }
}
