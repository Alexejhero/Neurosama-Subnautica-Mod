using NaughtyAttributes;
using UnityEngine;

public class SoundOnDamage : MonoBehaviour
{
    public DamageType damageType = DamageType.Collide;

    [Foldout(STRINGS.ASSIGNED_AT_RUNTIME), ReadOnly] public FMODAsset sound;
}
