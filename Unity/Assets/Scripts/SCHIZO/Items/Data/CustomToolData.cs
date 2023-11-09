using System;
using UnityEngine;
using TriInspector;
using SCHIZO.Interop.Subnautica.Enums;

namespace SCHIZO.Items.Data
{
    [Serializable]
    [DeclareBoxGroup("ik_bz", Title = "IK Overrides (Below Zero)")]
    public sealed class CustomToolData
    {
        public TechType_All referenceAnimation;
        public GameObject subnauticaModel;
        public GameObject belowZeroModel;

        [Group("ik_bz")] public Transform leftHandIKTargetOverrideBZ;
        [Group("ik_bz")] public Transform rightHandIKTargetOverrideBZ;
    }
}