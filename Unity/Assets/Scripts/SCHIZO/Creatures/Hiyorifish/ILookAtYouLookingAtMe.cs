using TriInspector;
using UnityEngine;

namespace SCHIZO.Creatures.Hiyorifish
{
    /// <summary>
    /// Turns the player's camera away from or towards this object.<br/>
    /// The effect is diminished with distance and looking angle.
    /// </summary>
    public partial class ILookAtYouLookingAtMe : MonoBehaviour
    {
        [InfoBox("Turns the player's camera away from or towards the target. The effect is diminished with distance and looking angle.")]
        [Tooltip("Maximum range at which to apply this effect. Effect strength decay from distance is quadratic.")]
        public float maxRange = 50;
        [Range(-10, 10), Tooltip("(Negative/positive) values will turn the camera (away from/towards) this object.")]
        public float lookTurnPower = -3;
    }
}
