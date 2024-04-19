using SCHIZO.Attributes;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Creatures.Hiyorifish
{
    /// <summary>
    /// Turns the player's camera away from or towards this object.<br/>
    /// The effect is diminished with world distance (between the player and this object)
    /// and looking angle distance (between where the camera's pointing and this object).
    /// </summary>
    public partial class ILookAtYouLookingAtMe : MonoBehaviour
    {
        [InfoBox("Turns the player's camera away from or towards the target. The effect is diminished with world distance (between the player and this object) and looking angle distance (between where the camera's pointing and this object).")]
        [Tooltip("Maximum range at which to apply this effect. Falloff from distance is quadratic.")]
        public float maxRange = 50;
        [Range(-10, 10), Tooltip("(Negative/positive) values will turn the camera (away from/towards) this object.")]
        public float lookTurnPower = -3;
        [InfoBox("Vanishes on leaving camera frame")]
        [Tooltip("Must have been active for at least this many seconds to vanish")]
        public float vanishMinLifetime = 60f;
        public float vanishChance = 0.5f;
        [InfoBox("Switches between \"passive\" and \"aggressive\" on a timer")]
        [Tooltip("Duration of passive mode (pushes view away, swims randomly)")]
        public float passiveDuration = 30f;
        [Tooltip("Duration of aggressive mode (pulls view, swims towards player, teleports behind their back)")]
        public float aggressiveDuration = 15f;
        [ExposedType("SwimBehaviour")]
        public MonoBehaviour swimBehaviour;
        public float swimSpeed = 8f;
        [Tooltip("On attack, waits this delay then teleports and deals damage")]
        public float teleportDelay = 2f;
        public float attackDamage = 10f;
        [Tooltip("After attack, idles for this long")]
        public float attackCooldown = 5f;
    }
}
