namespace SCHIZO.Unity.Creatures;

public sealed class CreaturePhysicMaterial : MonoBehaviour
{
    [InfoBox("If the physic material is left unset, it will default to a frictionless physic material.")]
    public PhysicMaterial physicMaterial;

    // TODO
    public void SetPhysicMaterial()
    {
        if (!physicMaterial) physicMaterial = new PhysicMaterial("NoFriction")
        {
            dynamicFriction = 0.0f,
            staticFriction = 0.0f,
            frictionCombine = PhysicMaterialCombine.Multiply,
            bounceCombine = PhysicMaterialCombine.Multiply
        };

        foreach (Collider componentsInChild in GetComponentsInChildren<Collider>(true))
            componentsInChild.sharedMaterial = physicMaterial;
    }
}
