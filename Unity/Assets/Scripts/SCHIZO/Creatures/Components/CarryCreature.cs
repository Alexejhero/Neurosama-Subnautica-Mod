using UnityEngine;
using NaughtyAttributes;
using SCHIZO.Interop.Subnautica.Enums;

namespace SCHIZO.Creatures.Components
{
    public sealed partial class CarryCreature : CustomCreatureAction
    {
        [SerializeField]
        private EcoTargetType_All _ecoTargetType;
        [Required]
        public Transform attachPoint;
        [Tooltip("Rotate the carried creature to match the attach point's forward/up vectors.")]
        public bool resetRotation;
        public float attachRadius = 2f;
        public float updateInterval = 2f;
        public float updateTargetInterval = 1f;
        public float swimVelocity = 10f;
        [Tooltip("Chance to drop the carried target, rolled every action update (every Update Interval)")]
        [Range(0f, 1f)]
        public float ADHD = 0.15f;
    }
}
