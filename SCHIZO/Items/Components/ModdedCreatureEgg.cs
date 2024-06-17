using ECCLibrary.Data;

namespace SCHIZO.Items.Components;

partial class ModdedCreatureEgg
{
    private CreatureEgg Egg => creatureEgg as CreatureEgg;
    private void Awake()
    {
        if (!Egg)
        {
            LOGGER.LogWarning($"{nameof(creatureEgg)} was not assigned to {this}, creating new - deserialization (e.g. loading hatching progress from save) will fail!");
            creatureEgg = gameObject.AddComponent<CreatureEgg>();
        }
        Egg.creaturePrefab = new CustomGameObjectReference(creature.GetClassID());
        Egg.creatureType = creature.GetTechType();
    }
}
