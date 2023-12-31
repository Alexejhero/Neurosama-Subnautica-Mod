using SCHIZO.Attributes;
using SCHIZO.TriInspector.Attributes;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Interop.Subnautica
{
    [DeclareComponentReferencesGroup]
    [DeclareUnexploredGroup]
    [DeclareBoxGroup("creatureaction", Title = "Base Creature Action")]
    [DeclareUnexploredGroup("creatureaction")]
    partial class _CreatureAction : MonoBehaviour
    {
        [ComponentReferencesGroup, Required, ExposedType("_Creature")] public MonoBehaviour creature;
        [ComponentReferencesGroup, Required, ExposedType("SwimBehaviour")] public MonoBehaviour swimBehaviour;

        [Group("creatureaction"), Range(0, 1)] public float evaluatePriority = 0.4f;

        [UnexploredGroup("creatureaction")] public AnimationCurve priorityMultiplier;
        [UnexploredGroup("creatureaction")] public float minActionCheckInterval = -1;
    }
}
