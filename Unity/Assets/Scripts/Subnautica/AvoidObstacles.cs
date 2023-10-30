using NaughtyAttributes;
using SCHIZO.Utilities;
using UnityEngine;

[RequireComponent(typeof(SwimBehaviour))]
public class AvoidObstacles : CreatureAction
{
    [Foldout(STRINGS.COMPONENT_REFERENCES)] public LastTarget lastTarget;

    public bool avoidTerrainOnly = true;
    public float avoidanceIterations = 10;
    public float avoidanceDistance = 5;
    public float avoidanceDuration = 2;
    public float scanInterval = 1;
    public float scanDistance = 2;
    public float scanRadius = 0;
    public float swimVelocity = 3;
    public float swimInterval = 1;
}
