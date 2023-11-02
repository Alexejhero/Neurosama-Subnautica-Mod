using JetBrains.Annotations;
using NaughtyAttributes;
using SCHIZO.Interop.Subnautica;
using SCHIZO.Sounds.Collections;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark
{
    public sealed partial class ErmsharkAttack : _MeleeAttack
    {
        [BoxGroup("Sounds"), SerializeField, Required, UsedImplicitly]
        private SoundCollectionInstance attackSounds;

        [BoxGroup("Sounds"), SerializeField, Required, UsedImplicitly]
        private _FMOD_CustomEmitter emitter;
    }
}
