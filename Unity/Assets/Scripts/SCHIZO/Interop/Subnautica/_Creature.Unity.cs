using SCHIZO.Attributes;
using SCHIZO.TriInspector;
using SCHIZO.TriInspector.Attributes;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Interop.Subnautica
{
    [DeclareUnexploredGroup(CREATURE_GROUP)]
    partial class _Creature : TriMonoBehaviour
    {
        [ComponentReferencesGroup, Required, ExposedType("LiveMixin")] public MonoBehaviour liveMixin;
        [ComponentReferencesGroup, Required] public Animator traitsAnimator;

        [Group(CREATURE_GROUP)] public AnimationCurve sizeDistribution;
        [Group(CREATURE_GROUP)] public _AggressionCreatureTrait Aggression = new _AggressionCreatureTrait(0, 0.05f);
        [Group(CREATURE_GROUP)] public _CreatureTrait Hunger = new _CreatureTrait(0, -0.1f);
        [Group(CREATURE_GROUP)] public _CreatureTrait Scared = new _CreatureTrait(0, 0.1f);
        [Group(CREATURE_GROUP), Range(0, -1)] public float eyeFOV = 0;

        [UnexploredGroup(CREATURE_GROUP)] public AnimationCurve initialCuriosity;
        [UnexploredGroup(CREATURE_GROUP)] public AnimationCurve initialFriendliness;
        [UnexploredGroup(CREATURE_GROUP)] public AnimationCurve initialHunger;
        [UnexploredGroup(CREATURE_GROUP)] public _CreatureTrait Curiosity;
        [UnexploredGroup(CREATURE_GROUP)] public _CreatureTrait Friendliness;
        [UnexploredGroup(CREATURE_GROUP)] public _CreatureTrait Tired;
        [UnexploredGroup(CREATURE_GROUP)] public _CreatureTrait Happy;
        [UnexploredGroup(CREATURE_GROUP)] public AnimationCurve activity;
        [UnexploredGroup(CREATURE_GROUP)] public bool hasEyes = true;
        [UnexploredGroup(CREATURE_GROUP)] public bool eyesOnTop = false;
        [UnexploredGroup(CREATURE_GROUP)] public float hearingSensitivity = 1;
        [UnexploredGroup(CREATURE_GROUP)] public bool detectsMotion = true;
        [UnexploredGroup(CREATURE_GROUP)] public float babyScaleSize = 0.5f;
    }
}
