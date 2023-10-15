using NaughtyAttributes;
using SCHIZO.Packages.NaughtyAttributes;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark
{
    public sealed partial class ErmsharkAttack
    {
        [BoxGroup("Sounds"), SerializeField, Required] private BaseSoundCollection attackSounds;
        [BoxGroup("Sounds"), SerializeField, Required, ValidateType("FMOD_CustomEmitter")] private MonoBehaviour emitter;
    }
}
