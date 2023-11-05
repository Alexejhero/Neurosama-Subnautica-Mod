using JetBrains.Annotations;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Creatures.Components
{
    public sealed partial class CreaturePhysicMaterial : MonoBehaviour
    {
        [InfoBox("If the physic material is left unset, it will default to a frictionless physic material.")]
        [SerializeField, UsedImplicitly]
        private PhysicMaterial physicMaterial;
    }
}
