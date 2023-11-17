using SCHIZO.Creatures.Components;
using SCHIZO.Interop.Subnautica;
using UnityEngine;

namespace SCHIZO.Creatures.Ermfish
{
    [RequireComponent(typeof(CarryCreature), typeof(Carryable), typeof(_SwimToTarget))]
    public sealed partial class ErmStack : MonoBehaviour
    {
        public int maxStack = 7;
        public CarryCreature socket;
        public Carryable plug;

        [Tooltip("Seconds between each update. Updating rolls the chances to start a stack/detach from a stack.")]
        public float updateInterval = 2f;

        [Tooltip("Chance (rolled each update) to target and approach the nearest of its kin.")]
        public float growStackChance = 0.10f;

        [Tooltip("Chance that this creature will detach if it's at the head (bottom) or tail (top) of the stack.")]
        public float endsDetachChance = 0.05f;
        [Tooltip("Chance that this creature will detach if it's in the middle of the stack.")]
        public float middleDetachChance = 0.02f;
    }
}
