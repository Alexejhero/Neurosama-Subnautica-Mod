using UnityEngine;

namespace SCHIZO.Creatures.Components;

partial class CreaturePhysicMaterial
{
    private void Awake()
    {
        if (!physicMaterial) physicMaterial = ECCLibrary.ECCUtility.FrictionlessPhysicMaterial;

        foreach (Collider componentsInChild in GetComponentsInChildren<Collider>(true))
            componentsInChild.sharedMaterial = physicMaterial;
    }
}
