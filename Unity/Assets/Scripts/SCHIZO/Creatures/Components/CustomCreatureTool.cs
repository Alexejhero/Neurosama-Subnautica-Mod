using NaughtyAttributes;
using SCHIZO.Interop.Subnautica;
using SCHIZO.Interop.Subnautica.Enums;
using UnityEngine;

namespace SCHIZO.Creatures.Components
{
    public partial class CustomCreatureTool : _CreatureTool
    {
        [SerializeField] private TechType_All referenceAnimation;
        [SerializeField] private GameObject subnauticaModel;
        [SerializeField] private GameObject belowZeroModel;

        [Foldout("IK"), SerializeField] private Transform leftHandIKTargetOverrideBZ;
        [Foldout("IK"), SerializeField] private Transform rightHandIKTargetOverrideBZ;
    }
}
