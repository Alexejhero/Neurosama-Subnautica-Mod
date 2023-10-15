using NaughtyAttributes;
using SCHIZO.Unity.NaughtyExtensions;
using SCHIZO.Unity.Sounds;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Creatures.Ermshark
{
    public sealed partial class ErmsharkAttack
    {
        [BoxGroup("Sounds"), SerializeField, Required] private BaseSoundCollection attackSounds;
        [BoxGroup("Sounds"), SerializeField, Required, ValidateType("FMOD_CustomEmitter")] private MonoBehaviour emitter;
    }
}
