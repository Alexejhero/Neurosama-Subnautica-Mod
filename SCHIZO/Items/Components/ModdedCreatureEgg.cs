using Nautilus.Extensions;

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

        string creatureId = creature.GetClassID();
        if (string.IsNullOrEmpty(creatureId))
        {
            LOGGER.LogError($"No creature assigned to egg {this}, destroying");
            Destroy(this);
            return;
        }
        Egg.creaturePrefab = new(creatureId);
        Egg.creaturePrefab.ForceValid();
        Egg.creatureType = creature.GetTechType();
    }
}
