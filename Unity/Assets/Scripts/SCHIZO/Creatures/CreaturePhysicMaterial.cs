using NaughtyAttributes;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Creatures
{
    public sealed class CreaturePhysicMaterial : MonoBehaviour
    {
        [InfoBox("If the physic material is left unset, it will default to a frictionless physic material.")]
        public PhysicMaterial physicMaterial;

#if !UNITY
        private void Awake()
        {
            if (!physicMaterial) physicMaterial = ECCLibrary.ECCUtility.FrictionlessPhysicMaterial;

            foreach (Collider componentsInChild in GetComponentsInChildren<Collider>(true))
                componentsInChild.sharedMaterial = physicMaterial;
        }
#endif
    }
}
