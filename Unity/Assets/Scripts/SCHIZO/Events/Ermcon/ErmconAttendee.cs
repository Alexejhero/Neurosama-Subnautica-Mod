using NaughtyAttributes;
using SCHIZO.Attributes.Typing;
using SCHIZO.Creatures.Components;
using SCHIZO.Interop.Subnautica;
using SCHIZO.Sounds;
using SCHIZO.Sounds.Players;
using UnityEngine;

namespace SCHIZO.Events.Ermcon
{
    public sealed partial class ErmconAttendee : CustomCreatureAction
    {
        [ValidateInput(nameof(ForceDisable))]
        [Range(1f, 100f)]
        [Tooltip("Affects how long the creature will stay focused on one target as well as the con itself. The relationship is non-linear.")]
        public float patience = 10f;

        [SerializeField] private SoundCollection pickupDeniedSounds;
        [SerializeField] private _FMOD_CustomEmitter emitter;

        private bool ForceDisable()
        {
            enabled = false;
            return true;
        }
    }
}
