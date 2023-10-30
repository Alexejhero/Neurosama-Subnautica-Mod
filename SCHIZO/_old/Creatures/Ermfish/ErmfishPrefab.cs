using System.Collections;
using ECCLibrary;
using ECCLibrary.Data;
using ECCLibrary.Mono;
using SCHIZO.Extensions;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Ermfish;

public class ErmfishPrefab : PickupableCreaturePrefab<Creature>
{
    public override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
    {
        CreatureSounds sounds = CreatureSoundsHandler.GetCreatureSounds(ModItems.Ermfish);
        InventorySounds.Add(prefab, sounds.AmbientItemSounds);
        WorldSounds.Add(prefab, sounds.AmbientWorldSounds);

        prefab.EnsureComponentFields();

        yield break;
    }
}
