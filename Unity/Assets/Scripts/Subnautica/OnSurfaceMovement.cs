using NaughtyAttributes;
using SCHIZO.Utilities;
using UnityEngine;

public class OnSurfaceMovement : MonoBehaviour
{
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public OnSurfaceTracker onSurfaceTracker;
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public Locomotion locomotion;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public float targetRange = 1;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public float onSurfaceDelay;
}
