using FMODUnity;
using JetBrains.Annotations;
using SCHIZO.Creatures.Components;
using SCHIZO.Interop.Subnautica;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Events.Ermcon
{
    public sealed partial class ErmconAttendee : CustomCreatureAction
    {
        [ValidateInput(nameof(ForceDisable))]
        [Range(1f, 100f)]
        [Tooltip("Affects how long the creature will stay focused on one target as well as the con itself. The relationship is non-linear.")]
        public float patience = 10f;

        [SerializeField, UsedImplicitly, EventRef]
        private string pickupDeniedSounds;
        [SerializeField, UsedImplicitly] private _FMOD_CustomEmitter emitter;

        private TriValidationResult ForceDisable()
        {
            enabled = false;
            return TriValidationResult.Valid;
        }
    }
}
