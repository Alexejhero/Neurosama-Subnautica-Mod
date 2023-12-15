using JetBrains.Annotations;
using SCHIZO.Attributes;
using SCHIZO.Options.Bool;
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

        [SerializeField, UsedImplicitly]
        private ToggleModOption enabledOption;

        [Space]

        [SerializeField, ExposedType("Pickupable"), UsedImplicitly]
        private MonoBehaviour pickupable;

        [SerializeField, ExposedType("Constructable"), UsedImplicitly]
        private MonoBehaviour constructable;

        protected override bool Is3D => true;
    }
}
