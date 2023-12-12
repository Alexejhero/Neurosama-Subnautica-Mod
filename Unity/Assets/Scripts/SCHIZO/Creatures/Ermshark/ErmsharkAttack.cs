using FMODUnity;
using JetBrains.Annotations;
using SCHIZO.Interop.Subnautica;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark
{
    [DeclareBoxGroup("Sounds")]
    public sealed partial class ErmsharkAttack : _MeleeAttack
    {
        [Group("Sounds"), SerializeField, Required, UsedImplicitly, EventRef]
        private string attackSounds;

        [Group("Sounds"), SerializeField, Required, UsedImplicitly]
        private _FMOD_CustomEmitter emitter;
    }
}
