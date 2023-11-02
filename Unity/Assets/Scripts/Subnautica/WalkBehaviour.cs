using NaughtyAttributes;
using SCHIZO.Utilities;
using UnityEngine;

[RequireComponent(typeof(OnSurfaceMovement))]
public class WalkBehaviour : SwimBehaviour
{
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public OnSurfaceTracker onSurfaceTracker;
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public OnSurfaceMovement onSurfaceMovement;
    public bool allowSwimming;
}
