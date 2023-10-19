using UnityEngine;
using NaughtyAttributes;

namespace SCHIZO.Creatures.Components
{
    public partial class CarryCreature : CustomCreatureAction
    {
        [Required]
        public Transform attachPoint;
        public float attachRadius = 2f;
        public float updateInterval = 2f;
        public float updateTargetInterval = 1f;
        public float swimVelocity = 10f;
        [Tooltip("Chance to drop the carried target, rolled every action update (every Update Interval)")]
        [Range(0f, 1f)]
        public float ADHD = 0.15f;
    }
}
