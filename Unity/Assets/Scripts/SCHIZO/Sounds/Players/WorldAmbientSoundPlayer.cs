using JetBrains.Annotations;
using SCHIZO.Attributes;
using SCHIZO.Options.Float;
using UnityEngine;

namespace SCHIZO.Sounds.Players
{
    public sealed partial class WorldAmbientSoundPlayer : SoundPlayer
    {
        [Space]

        [SerializeField, UsedImplicitly]
        private ConfigurableValueFloat MinDelay;

        [SerializeField, UsedImplicitly]
        private ConfigurableValueFloat MaxDelay;

        [Space]

        [SerializeField, ExposedType("Pickupable"), UsedImplicitly]
        private MonoBehaviour pickupable;

        [SerializeField, ExposedType("Constructable"), UsedImplicitly]
        private MonoBehaviour constructable;

        protected override bool Is3D => true;
    }
}
