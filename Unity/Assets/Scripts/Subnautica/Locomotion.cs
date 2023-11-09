using SCHIZO.TriInspector.Attributes;
using TriInspector;
using UnityEngine;

[DeclareComponentReferencesGroup]
[DeclareUnexploredGroup]
public class Locomotion : MonoBehaviour
{
    [ComponentReferencesGroup, Required] public BehaviourLOD levelOfDetail;
    [ComponentReferencesGroup, Required] public Rigidbody useRigidbody;

    public float maxAcceleration = 10;
    public float forwardRotationSpeed = 0.6f;
    public float upRotationSpeed = 3;
    [Range(0.0f, 1f)] public float driftFactor = 0.5f;
    public bool canMoveAboveWater = false;
    public bool canWalkOnSurface = false;
    public bool freezeHorizontalRotation = false;

    [UnexploredGroup] public float maxVelocity = 10;
    [UnexploredGroup] public bool rotateToSurfaceNormal = true;
}
