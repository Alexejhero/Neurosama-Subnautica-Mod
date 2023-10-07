using UnityEngine;

[RequireComponent(typeof(SwimBehaviour))]
public class AvoidTerrain : CreatureAction
{
    [Range(0.0f, 1f)] public float avoidanceForward = 0.5f;
    public float avoidanceIterations = 10;
    public float avoidanceDistance = 50;
    public float avoidanceDuration = 2;
    public float scanInterval = 1;
    public float scanDistance = 30;
    public float swimVelocity = 15;
    public float swimInterval = 1;
}
