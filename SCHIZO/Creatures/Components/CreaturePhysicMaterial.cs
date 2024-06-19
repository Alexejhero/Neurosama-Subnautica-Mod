using UnityEngine;

namespace SCHIZO.Creatures.Components;

partial class CreaturePhysicMaterial
{
    private static readonly PhysicMaterial _frictionless = new()
    {
        dynamicFriction = 0,
        staticFriction = 0,
        frictionCombine = PhysicMaterialCombine.Multiply,
        bounceCombine = PhysicMaterialCombine.Multiply,
    };
    private void Awake()
    {
        if (!physicMaterial) physicMaterial = _frictionless;

        foreach (Collider componentsInChild in GetComponentsInChildren<Collider>(true))
            componentsInChild.sharedMaterial = physicMaterial;
    }
}
