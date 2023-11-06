using SCHIZO.TriInspector.Attributes;
using TriInspector;
using UnityEngine;

[DeclareComponentReferencesGroup]
public class SwimBehaviour : MonoBehaviour
{
    [ComponentReferencesGroup, Required] public SplineFollowing splineFollowing;

    [Range(0.0f, 1f)] public float turnSpeed = 1;
}
