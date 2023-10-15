using NaughtyAttributes;
using SCHIZO.Unity.NaughtyExtensions;
using SCHIZO.Unity.Sounds;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Creatures
{
    public sealed partial class HurtSoundPlayer : MonoBehaviour
    {
        [Required, SerializeField] private BaseSoundCollection hurtSounds;
        [Required, SerializeField, ValidateType("FMOD_CustomEmitter")] private MonoBehaviour emitter;
    }
}
