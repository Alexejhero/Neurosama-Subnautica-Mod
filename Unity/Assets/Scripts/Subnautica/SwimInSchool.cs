using UnityEngine;

[RequireComponent(typeof(SwimBehaviour))]
public class SwimInSchool : CreatureAction
{
    public float schoolSize = 2;
    public float swimVelocity = 4;
    public float swimInterval = 1;
}
