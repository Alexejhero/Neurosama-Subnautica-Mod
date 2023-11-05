using NaughtyAttributes;
using SCHIZO.Interop.Subnautica;
using SCHIZO.Utilities;
using UnityEngine;

[RequireComponent(typeof(SwimBehaviour))]
[RequireComponent(typeof(CreatureFear))]
public class FleeWhenScared : CreatureAction
{
    [Foldout(STRINGS.COMPONENT_REFERENCES), Required] public CreatureFear creatureFear;

    public float swimVelocity = 10;
    public float swimInterval = 1;
    public int avoidanceIterations = 10;

    public float swimTiredness = 0.2f;
    public float tiredVelocity = 3;

    public _CreatureTrait exhausted = new _CreatureTrait(0, 0.05f);
    public float swimExhaustion = 0.25f;
    public float exhaustedVelocity = 1;

    [Foldout(STRINGS.UNCHANGED_BY_ECC)] public FMODAsset scaredSound;
}
