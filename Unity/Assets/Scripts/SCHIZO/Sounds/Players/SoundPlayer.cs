using FMODUnity;
using JetBrains.Annotations;
using SCHIZO.Interop.Subnautica;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Sounds.Players
{
    public abstract partial class SoundPlayer : MonoBehaviour
    {
        [EventRef, SerializeField, Required, UsedImplicitly]
        protected string soundEvent;

        [SerializeField, UsedImplicitly]
        [Required(Message ="Players without an emitter cannot \"own\" sounds and therefore will not be able to stop sounds that are playing")]
        private _FMOD_CustomEmitter emitter;

        protected abstract bool Is3D { get; }
    }
}
