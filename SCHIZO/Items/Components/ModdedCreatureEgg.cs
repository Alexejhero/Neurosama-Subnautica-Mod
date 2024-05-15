using ECCLibrary.Data;

namespace SCHIZO.Items.Components;

partial class ModdedCreatureEgg
{
    private void Awake()
    {
        CreatureEgg egg = gameObject.AddComponent<CreatureEgg>();
        egg.creaturePrefab = new CustomGameObjectReference(creature.GetClassID());
        egg.creatureType = creature.GetTechType();
        egg.daysBeforeHatching = daysBeforeHatching;
        egg.animator = animator;
        egg.liveMixin = (LiveMixin)liveMixin;
        // TODO: hatching progress deserialization
    }
}
