using FMODUnity;
using JetBrains.Annotations;
using SCHIZO.Interop.Subnautica;
using TriInspector;
using UnityEngine;
using SCHIZO.Helpers;

namespace SCHIZO.Sounds.Players
{
    public abstract partial class SoundPlayer : MonoBehaviour
    {
        [EventRef, SerializeField, Required, UsedImplicitly]
        protected string soundEvent;

        [SerializeField, ShowIf(nameof(Is3D)), Required, UsedImplicitly]
        private _FMOD_CustomEmitter emitter;

        public FmodEventInstanceUnityEvent onPlay;

        protected abstract bool Is3D { get; }
    }
}
