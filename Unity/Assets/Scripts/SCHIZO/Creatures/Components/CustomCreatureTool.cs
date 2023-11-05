using TriInspector;
using SCHIZO.Interop.Subnautica;
using SCHIZO.Interop.Subnautica.Enums;
using UnityEngine;

namespace SCHIZO.Creatures.Components
{
    [DeclareBoxGroup("ik_bz", Title = "IK Overrides (Below Zero)")]
    public partial class CustomCreatureTool : _CreatureTool
    {
        [SerializeField] private TechType_All referenceAnimation;
        [SerializeField] private GameObject subnauticaModel;
        [SerializeField] private GameObject belowZeroModel;

        [Group("ik_bz"), LabelText("Left Hand IK Target"), SerializeField] private Transform leftHandIKTargetOverrideBZ;
        [Group("ik_bz"), LabelText("Right Hand IK Target"), SerializeField] private Transform rightHandIKTargetOverrideBZ;
    }
}
