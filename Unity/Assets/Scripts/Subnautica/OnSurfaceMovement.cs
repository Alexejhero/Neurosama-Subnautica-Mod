using SCHIZO.TriInspector.Attributes;
using TriInspector;
using UnityEngine;

[DeclareComponentReferencesGroup]
[DeclareUnexploredGroup]
public class OnSurfaceMovement : MonoBehaviour
{
    [ComponentReferencesGroup, Required] public OnSurfaceTracker onSurfaceTracker;
    [ComponentReferencesGroup, Required] public Locomotion locomotion;
    [UnexploredGroup] public float targetRange = 1;
    [UnexploredGroup] public float onSurfaceDelay;
}
