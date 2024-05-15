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
#if BELOWZERO
        egg.animators = [animator];
        egg.animateOutside = true; // todo
#else
        egg.animator = animator;
#endif
        egg.liveMixin = (LiveMixin)liveMixin;
        // TODO: hatching progress deserialization
    }
}
