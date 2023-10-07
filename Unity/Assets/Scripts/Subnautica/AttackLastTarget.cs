using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(LastTarget))]
[RequireComponent(typeof(SwimBehaviour))]
public class AttackLastTarget : CreatureAction
{
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public LastTarget lastTarget;

    public float swimVelocity = 10;
    public float aggressionThreshold = 0.75f;
    public float minAttackDuration = 3;
    public float maxAttackDuration = 7;
    public float pauseInterval = 20;
    public bool resetAggressionOnTime = true;
    public float rememberTargetTime = 5;

    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public float swimInterval = 0.8f;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public float swimIntoPlayerViewMinDistance = 0;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool ignoreAboveWaterTargets = false;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool ignoreUnderWaterTargets = false;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool predictTargetMovement = false;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool attackStartRoar = false;
    [Foldout(STRINGS.UNCHANGED_BY_ECC), ReadOnly] public Object _attackStartSound;
    [Foldout(STRINGS.UNCHANGED_BY_ECC), ReadOnly] public Object _attackLoopSound;
    [Foldout(STRINGS.UNCHANGED_BY_ECC), ReadOnly] public Object _attackStartFXcontrol;
}
