using System.Linq;
using JetBrains.Annotations;
using SCHIZO.Attributes;
using SCHIZO.Options.Bool;
using SCHIZO.Options.Float;
using TriInspector;
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

        [SerializeField, UsedImplicitly]
        private ToggleModOption enabledOption;

        [Space]

        [SerializeField, ExposedType("Pickupable"), Required, UsedImplicitly, ValidateInput(nameof(Validate_pickupable))]
        private MonoBehaviour pickupable;

        protected override bool Is3D => false;

        private TriValidationResult Validate_pickupable()
        {
            if (!pickupable) return TriValidationResult.Error("Pickupable is required!");
            if (!pickupable.GetComponentsInChildren<InventoryAmbientSoundPlayer>().Contains(this)) return TriValidationResult.Error("Pickupable component must be a parent of this sound player!");
            return TriValidationResult.Valid;
        }
    }
}
