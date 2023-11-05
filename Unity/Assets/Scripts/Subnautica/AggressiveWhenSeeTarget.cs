using SCHIZO.Interop.Subnautica;
using SCHIZO.Interop.Subnautica.Enums;
using TriExtensions;
using TriInspector;
using UnityEngine;

[RequireComponent(typeof(LastTarget))]
[RequireComponent(typeof(CreatureFear))]
public class AggressiveWhenSeeTarget : TriMonoBehaviour
{
    [ComponentReferencesGroup, Required] public LastTarget lastTarget;
    [ComponentReferencesGroup, Required] public _Creature creature;

    public AnimationCurve maxRangeMultiplier = DefaultMaxRangeMultiplier;
    public AnimationCurve distanceAggressionMultiplier = DefaultDistanceAggressionMultiplier;
    [InfoBox("If Target Type is not set to None, other targets will be attacked if the player cannot be attacked.", visibleIf: nameof(ShowPlayerAttackInfobox))]
    public EcoTargetType_All targetType = EcoTargetType_All.Shark;
    public float aggressionPerSecond = 1;
    public float maxRangeScalar = 10;
    public int maxSearchRings = 1;
    public bool ignoreSameKind = true;
    public float minimumVelocity = 0;
    public float hungerThreshold = 0;

    [UnexploredGroup] public bool ignoreFrozen = false;
    [UnexploredGroup] public float leashDistance = -1;
    [UnexploredGroup] public float targetPriority = 1;
    [UnexploredGroup] public FMODAsset sightedSound;

    protected virtual bool ShowPlayerAttackInfobox() => false;

    private static AnimationCurve DefaultMaxRangeMultiplier => new AnimationCurve(new Keyframe[]
    {
        new Keyframe(0.0f, 1f, 0.0f, 0.0f, 0.333f, 0.333f),
        new Keyframe(0.5f, 0.5f, 0.0f, 0.0f, 0.333f, 0.333f),
        new Keyframe(1f, 1f, 0.0f, 0.0f, 0.333f, 0.333f)
    });

    private static AnimationCurve DefaultDistanceAggressionMultiplier => new AnimationCurve(new Keyframe[]
    {
        new Keyframe(0.0f, 1f, 0.0f, 0.0f, 0.333f, 0.333f),
        new Keyframe(1f, 0.0f, -3f, -3f, 0.333f, 0.333f)
    });
}
