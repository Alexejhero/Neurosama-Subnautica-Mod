using System;
using NaughtyAttributes;
using SCHIZO.Attributes.Typing;
using SCHIZO.Utilities;
using UnityEngine;

namespace SCHIZO.Interop.Subnautica
{
    partial class _Creature : MonoBehaviour
    {
        [Foldout(STRINGS.COMPONENT_REFERENCES), Required, ExposedType("LiveMixin")] public MonoBehaviour liveMixin;

        [Required] public Animator traitsAnimator;

        [BoxGroup("Base Creature")] public AnimationCurve sizeDistribution;
        [BoxGroup("Base Creature")] public AggressionCreatureTrait Aggression = new AggressionCreatureTrait(0, 0.05f);
        [BoxGroup("Base Creature")] public CreatureTrait Hunger = new CreatureTrait(0, -0.1f);
        [BoxGroup("Base Creature")] public CreatureTrait Scared = new CreatureTrait(0, 0.1f);
        [BoxGroup("Base Creature"), Range(0, -1)] public float eyeFOV = 0.25f;

        [Foldout(STRINGS.UNCHANGED_BY_ECC)] public AnimationCurve initialCuriosity;
        [Foldout(STRINGS.UNCHANGED_BY_ECC)] public AnimationCurve initialFriendliness;
        [Foldout(STRINGS.UNCHANGED_BY_ECC)] public AnimationCurve initialHunger;
        [Foldout(STRINGS.UNCHANGED_BY_ECC)] public CreatureTrait Curiosity;
        [Foldout(STRINGS.UNCHANGED_BY_ECC)] public CreatureTrait Friendliness;
        [Foldout(STRINGS.UNCHANGED_BY_ECC)] public CreatureTrait Tired;
        [Foldout(STRINGS.UNCHANGED_BY_ECC)] public CreatureTrait Happy;
        [Foldout(STRINGS.UNCHANGED_BY_ECC)] public AnimationCurve activity;
        [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool hasEyes = true;
        [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool eyesOnTop = false;
        [Foldout(STRINGS.UNCHANGED_BY_ECC)] public float hearingSensitivity = 1;
        [Foldout(STRINGS.UNCHANGED_BY_ECC)] public bool detectsMotion = true;
        [Foldout(STRINGS.UNCHANGED_BY_ECC)] public float babyScaleSize = 0.5f;

        [Serializable]
        public class CreatureTrait
        {
            public float value;
            public float falloff;

            public CreatureTrait(float value, float falloff)
            {
                this.value = value;
                this.falloff = falloff;
            }
        }

        [Serializable]
        public class AggressionCreatureTrait : CreatureTrait
        {
            public AggressionCreatureTrait(float value, float falloff) : base(value, falloff)
            {
            }
        }
    }
}
