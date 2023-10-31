using System;
using UnityEngine;
using NaughtyAttributes;
using SCHIZO.Interop.Subnautica.Enums;

namespace SCHIZO.Items.Data
{
    [Serializable]
    public sealed class CustomToolData
    {
        public TechType_All referenceAnimation;
        public GameObject subnauticaModel;
        public GameObject belowZeroModel;

        [Foldout("IK")] public Transform leftHandIKTargetOverrideBZ;
        [Foldout("IK")] public Transform rightHandIKTargetOverrideBZ;
    }
}