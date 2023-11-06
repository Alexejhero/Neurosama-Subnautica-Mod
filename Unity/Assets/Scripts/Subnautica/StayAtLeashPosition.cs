using SCHIZO.TriInspector.Attributes;
using UnityEngine;

[RequireComponent(typeof(SwimBehaviour))]
public class StayAtLeashPosition : CreatureAction
{
    public float leashDistance = 15;
    public float swimVelocity = 3;
    public float swimInterval = 1;
    public float minSwimDuration = 3;

    [UnexploredGroup] public Vector3 directionDistanceMultiplier = Vector3.one;
}
