using TriInspector;

public class MoveOnSurface : CreatureAction
{
    [ComponentReferencesGroup, Required] public OnSurfaceTracker onSurfaceTracker;
    [ComponentReferencesGroup, Required] public WalkBehaviour walkBehaviour;
    [ComponentReferencesGroup, Required] public OnSurfaceMovement onSurfaceMovement;
    [UnexploredGroup] public float updateTargetInterval = 5;
    [UnexploredGroup] public float updateTargetRandomInterval = 6;
    [UnexploredGroup] public float moveVelocity = 13;
    [UnexploredGroup] public float moveRadius = 7;
    [UnexploredGroup] public bool moveOnWalls;
}
