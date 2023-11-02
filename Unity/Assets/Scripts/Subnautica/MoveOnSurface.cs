using NaughtyAttributes;
using SCHIZO.Utilities;

public class MoveOnSurface : CreatureAction
{
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public OnSurfaceTracker onSurfaceTracker;
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public WalkBehaviour walkBehaviour;
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public OnSurfaceMovement onSurfaceMovement;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public float updateTargetInterval = 5;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public float updateTargetRandomInterval = 6;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public float moveVelocity = 13;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public float moveRadius = 7;
    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool moveOnWalls;
}
