using System.Collections;
using ECCLibrary;
using ECCLibrary.Data;
using SCHIZO.Extensions;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark;

public sealed class ErmsharkPrefab : CustomCreaturePrefab<ErmsharkBehaviour>
{
    public static GameObject Prefab;

    public ErmsharkPrefab(ModItem modItem, GameObject creaturePrefab) : base(modItem, creaturePrefab)
    {
    }

    public override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
    {
        BullyTutel bully = prefab.AddComponent<BullyTutel>();
        bully.mouth = bully.tutelAttach = prefab.SearchChild("mouth_attach_point").transform;

        Prefab = prefab;

        yield break;
    }
}
