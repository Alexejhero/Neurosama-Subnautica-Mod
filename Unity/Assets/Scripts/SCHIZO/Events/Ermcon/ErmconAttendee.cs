using NaughtyAttributes;
using SCHIZO.Attributes.Typing;
using SCHIZO.Creatures.Components;
using SCHIZO.Sounds;
using UnityEngine;
using UnityEngine.Serialization;

namespace SCHIZO.Events.Ermcon
{
    public sealed partial class ErmconAttendee : CustomCreatureAction
    {
        [ValidateInput(nameof(ForceDisable))]
        [Range(1f, 100f)]
        [Tooltip("Affects how long the creature will stay focused on one target as well as the con itself. The relationship is non-linear.")]
        public float patience = 10f;

        [SerializeField] private GenericSoundPlayer pickupDeniedSoundPlayer;

        private bool ForceDisable()
        {
            enabled = false;
            return true;
        }
    }
}
