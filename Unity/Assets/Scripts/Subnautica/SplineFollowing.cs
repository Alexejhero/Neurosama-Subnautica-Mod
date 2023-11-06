using SCHIZO.TriInspector;
using SCHIZO.TriInspector.Attributes;
using TriInspector;
using UnityEngine;

public class SplineFollowing : TriMonoBehaviour
{
    [ComponentReferencesGroup, Required] public Locomotion locomotion;
    [ComponentReferencesGroup, Required] public Rigidbody useRigidbody;
    [ComponentReferencesGroup, Required] public BehaviourLOD levelOfDetail;

    [UnexploredGroup] public float targetRange = 1;
    [UnexploredGroup] public float lookAhead = 1;
    [UnexploredGroup] public bool respectLOD = true;

    [HideInInspector] public float inertia = 1;
}
