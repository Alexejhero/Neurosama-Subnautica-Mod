using System.Collections;
using ECCLibrary;
using ECCLibrary.Data;
using SCHIZO.Extensions;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark;

public sealed class ErmsharkPrefab : CustomCreaturePrefab<ErmsharkBehaviour>
{
    public const float swimVelocity = 8f;

    public static GameObject Prefab;

    public ErmsharkPrefab(ModItem modItem, GameObject creaturePrefab) : base(modItem, creaturePrefab)
    {
    }

    public override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
    {
        WorldSounds.Add(prefab, CreatureSoundsHandler.GetCreatureSounds(ModItems.Ermshark).AmbientWorldSounds);

        BullyTutel bully = prefab.AddComponent<BullyTutel>();
        bully.mouth = bully.tutelAttach = prefab.SearchChild("mouth_attach_point").transform;

        GameObject mouth = prefab.SearchChild("attack_collider");
        CreaturePrefabUtils.AddMeleeAttack<ErmsharkAttack>(prefab, components, mouth, true, 20);

        Prefab = prefab;

        yield break;
    }
}
