using TriInspector;
using UnityEngine;

public class SoundOnDamage : MonoBehaviour
{
    [InfoBox("This class is only used to provide the collision sound that plays when ramming into a fish with a seamoth or another vehicle. Currently there is no functionality for any other types of damage.")]
    [ReadOnly] public DamageType damageType = DamageType.Collide;
    [ReadOnly] public FMODAsset sound;
}
