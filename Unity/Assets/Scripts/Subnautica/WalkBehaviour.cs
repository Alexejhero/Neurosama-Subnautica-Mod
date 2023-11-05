using TriInspector;
using UnityEngine;

[RequireComponent(typeof(OnSurfaceMovement))]
public class WalkBehaviour : SwimBehaviour
{
    [ComponentReferencesGroup, Required] public OnSurfaceTracker onSurfaceTracker;
    [ComponentReferencesGroup, Required] public OnSurfaceMovement onSurfaceMovement;
    public bool allowSwimming;
}
