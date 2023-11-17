using UnityEngine;
using TriInspector;
using JetBrains.Annotations;
using SCHIZO.Interop.Subnautica.Enums;
using SCHIZO.Interop.Subnautica;
using SCHIZO.TriInspector.Attributes;

namespace SCHIZO.Creatures.Components
{
    [DeclareComponentReferencesGroup]
    public sealed partial class CarryCreature : MonoBehaviour
    {
        [ComponentReferencesGroup]
        public _SwimToTarget swimToTarget;
        [SerializeField, UsedImplicitly]
        [Tooltip("None means this won't seek out carryables at all - then, it can only pick anything up from the player's hands"
            + "\n(or if directed by another component)")]
        private EcoTargetType_All _ecoTargetType;
        public float attachRadius = 2f;
        public float updateInterval = 2f;
        public float updateTargetInterval = 1f;

        [Required]
        public Transform attachSocket;
        [Tooltip("Rotate the carried creature to match the attach point's forward/up vectors.")]
        public bool resetRotation;
        [Tooltip("Chance to drop the carried target, rolled every action update (every Update Interval)")]
        [Range(0f, 1f)]
        public float ADHD = 0.15f;
    }
}
