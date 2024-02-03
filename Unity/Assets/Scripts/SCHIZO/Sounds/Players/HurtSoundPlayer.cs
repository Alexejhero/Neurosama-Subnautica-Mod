using FMODUnity;
using JetBrains.Annotations;
using UnityEngine;

namespace SCHIZO.Sounds.Players
{
    public sealed partial class HurtSoundPlayer : SoundPlayer
    {
        protected override bool Is3D => true;
        [EventRef, SerializeField, UsedImplicitly]
        private string deathSound;
    }
}
