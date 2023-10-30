using JetBrains.Annotations;
using NaughtyAttributes;
using SCHIZO.Attributes.Typing;
using UnityEngine;

namespace SCHIZO.Sounds.Players
{
    public partial class InventoryAmbientSoundPlayer : SoundPlayer
    {
        [Space]

        [SerializeField, ExposedType("Pickupable"), Required, UsedImplicitly, ValidateInput(nameof(Validate_pickupable), "Pickupable component must be on the same GameObject as the sound player!")]
        private MonoBehaviour pickupable;

        protected override string DefaultBus => buses[PDA_VOICE];
        protected override bool Is3D => false;

        // ReSharper disable once Unity.PreferGenericMethodOverload
        private bool Validate_pickupable(MonoBehaviour behaviour) => behaviour == GetComponent("Pickupable");
    }
}
