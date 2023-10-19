using NaughtyAttributes;
using SCHIZO.Attributes.Typing;
using SCHIZO.Utilities;
using UnityEngine;

namespace SCHIZO.Interop.Subnautica
{
    partial class _CreatureAction : MonoBehaviour
    {
        [Foldout(STRINGS.COMPONENT_REFERENCES), Required, ValidateType("Creature")] public MonoBehaviour creature;
        [Foldout(STRINGS.COMPONENT_REFERENCES), Required, ValidateType("SwimBehaviour")] public MonoBehaviour swimBehaviour;

        [BoxGroup("Base Creature Action"), Range(0, 1)] public float evaluatePriority = 0.4f;

        [Foldout(STRINGS.UNCHANGED_BY_ECC)] public AnimationCurve priorityMultiplier;
        [Foldout(STRINGS.UNCHANGED_BY_ECC)] public float minActionCheckInterval = -1;
    }
}
