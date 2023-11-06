using JetBrains.Annotations;
using SCHIZO.Interop.Subnautica;
using SCHIZO.Sounds.Collections;
using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace SCHIZO.Sounds.Players
{
    public abstract partial class SoundPlayer : MonoBehaviour
    {
        [FormerlySerializedAs("soundCollection_"), SerializeField, Required, UsedImplicitly]
        protected SoundCollectionInstance sounds;

        [SerializeField, ShowIf(nameof(Is3D)), Required, UsedImplicitly]
        private _FMOD_CustomEmitter emitter;

        protected abstract bool Is3D { get; }
    }
}
