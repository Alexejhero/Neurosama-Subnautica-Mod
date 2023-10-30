using NaughtyAttributes;
using SCHIZO.Utilities;
using UnityEngine;

public class SwimBehaviour : MonoBehaviour
{
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public SplineFollowing splineFollowing;

    [Range(0.0f, 1f)] public float turnSpeed = 1;
}
