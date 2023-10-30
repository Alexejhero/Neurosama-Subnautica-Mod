using UnityEngine;

[RequireComponent(typeof(SwimBehaviour))]
[DisallowMultipleComponent]
public class SwimRandom : CreatureAction
{
    public Vector3 swimRadius = new Vector3(10, 2, 10);
    public float swimForward = 0.5f;
    public float swimVelocity = 2f;
    public float swimInterval = 5f;
    public bool onSphere = false;
}
