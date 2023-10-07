using NaughtyAttributes;
using UnityEngine;

public class Locomotion : MonoBehaviour
{
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public BehaviourLOD levelOfDetail;
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public Rigidbody useRigidbody;

    public float maxAcceleration = 10;
    public float forwardRotationSpeed = 0.6f;
    public float upRotationSpeed = 3;
    [Range(0.0f, 1f)] public float driftFactor = 0.5f;
    public bool canMoveAboveWater = false;
    public bool canWalkOnSurface = false;
    public bool freezeHorizontalRotation = false;

    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public float maxVelocity = 10;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool rotateToSurfaceNormal = true;
}
