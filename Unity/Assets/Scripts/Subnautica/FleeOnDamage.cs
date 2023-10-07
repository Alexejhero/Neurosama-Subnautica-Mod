using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(SwimBehaviour))]
public class FleeOnDamage : CreatureAction
{
    public float damageThreshold = 10;
    public float fleeDuration = 2;
    public float minFleeDistance = 5;
    public float swimVelocity = 10;
    public float swimInterval = 1;

    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool breakLeash = true;
}
