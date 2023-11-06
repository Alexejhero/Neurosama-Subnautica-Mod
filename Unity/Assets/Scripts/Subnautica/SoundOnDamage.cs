using TriInspector;
using UnityEngine;

// Note: this class is only used to provide the collision sound that plays when ramming into a fish with a seamoth or another vehicle
// currently there is no functionality for any other types of damage
public class SoundOnDamage : MonoBehaviour
{
    [ReadOnly]
    public DamageType damageType = DamageType.Collide;

    // [Foldout(STRINGS.ASSIGNED_AT_RUNTIME), ReadOnly] public FMODAsset sound;
}
