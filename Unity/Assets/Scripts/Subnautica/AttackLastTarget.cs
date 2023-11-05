using NaughtyAttributes;
using SCHIZO.Interop.Subnautica;
using SCHIZO.Utilities;
using UnityEngine;

[RequireComponent(typeof(LastTarget))]
[RequireComponent(typeof(SwimBehaviour))]
public class AttackLastTarget : CreatureAction
{
    [ComponentReferencesGroup, Required] public LastTarget lastTarget;

    public float swimVelocity = 10;
    public float aggressionThreshold = 0.75f;
    public float minAttackDuration = 3;
    public float maxAttackDuration = 7;
    public float pauseInterval = 20;
    public bool resetAggressionOnTime = true;
    public float rememberTargetTime = 5;

    [UnexploredGroup] public float swimInterval = 0.8f;
    [UnexploredGroup] public float swimIntoPlayerViewMinDistance = 0;
    [UnexploredGroup] public bool ignoreAboveWaterTargets = false;
    [UnexploredGroup] public bool ignoreUnderWaterTargets = false;
    [UnexploredGroup] public bool predictTargetMovement = false;
    [UnexploredGroup] public bool attackStartRoar = false;
    [UnexploredGroup] public _FMODAsset attackStartSound;
    [UnexploredGroup] public _FMODAsset attackLoopSound;
    [UnexploredGroup] public _FMODAsset attackStartFXcontrol;
}
