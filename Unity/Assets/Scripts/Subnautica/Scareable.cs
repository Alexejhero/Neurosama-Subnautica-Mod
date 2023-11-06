using SCHIZO.Interop.Subnautica;
using SCHIZO.Interop.Subnautica.Enums;
using SCHIZO.TriInspector.Attributes;
using TriInspector;
using UnityEngine;

[DeclareComponentReferencesGroup]
[DeclareUnexploredGroup]
public class Scareable : MonoBehaviour
{
    [ComponentReferencesGroup, Required] public CreatureFear creatureFear;
    [ComponentReferencesGroup, Required] public _Creature creature;

    public EcoTargetType_All targetType = EcoTargetType_All.Shark;
    public CreatureAction fleeAction;
    public float scarePerSecond = 4;
    public float maxRangeScalar = 10;
    public float minMass = 50;
    public float updateTargetInterval = 1;
    public float updateRange = 100;

    [UnexploredGroup] public AnimationCurve daynightRangeMultiplier;
}
