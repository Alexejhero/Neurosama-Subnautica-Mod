using NaughtyAttributes;
using SCHIZO.TriInspector;
using UnityEngine;

public class SwimBehaviour : TriMonoBehaviour
{
    [ComponentReferencesGroup, Required] public SplineFollowing splineFollowing;

    [Range(0.0f, 1f)] public float turnSpeed = 1;
}
