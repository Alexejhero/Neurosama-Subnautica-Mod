using System.Linq;
using JetBrains.Annotations;
using NaughtyAttributes;
using SCHIZO.Options.Float;
using SCHIZO.Attributes.Typing;
using UnityEngine;

namespace SCHIZO.Sounds.Players
{
    public partial class InventoryAmbientSoundPlayer : SoundPlayer
    {
        [Space]

        [SerializeField, UsedImplicitly]
        private ConfigurableValueFloat MinDelay;

        [SerializeField, UsedImplicitly]
        private ConfigurableValueFloat MaxDelay;

        [Space]

        [SerializeField, ExposedType("Pickupable"), Required, UsedImplicitly, ValidateInput(nameof(Validate_pickupable), "Pickupable component must be on the same GameObject as the sound player!")]
        private MonoBehaviour pickupable;

        protected override string DefaultBus => buses[PDA_VOICE];
        protected override bool Is3D => false;

        private bool Validate_pickupable() => !pickupable || pickupable.GetComponentsInChildren<InventoryAmbientSoundPlayer>().Contains(this);
    }
}
