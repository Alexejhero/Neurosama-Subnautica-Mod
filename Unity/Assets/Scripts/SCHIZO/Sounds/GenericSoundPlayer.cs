using UnityEngine;

namespace SCHIZO.Sounds
{
    public sealed class GenericSoundPlayer : SoundPlayer
    {
        [SerializeField] private bool is3D;

        protected override string DefaultBus => null;
        protected override bool Is3D => is3D;
    }
}
