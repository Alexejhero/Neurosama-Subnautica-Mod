using System.Collections;
using UnityEngine;

namespace SCHIZO.Helpers;
internal static class ObjectReferences
{
    public static bool Done { get; private set; }

    public static IEnumerator SetReferences()
    {
        TechType creatureReference;
#if SUBNAUTICA
        creatureReference = TechType.Peeper;
#elif BELOWZERO
        creatureReference = TechType.ArcticPeeper;
#endif
        CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(creatureReference);
        yield return task;
        GameObject peeper = task.GetResult();
        LiveMixinData lm = peeper.GetComponent<LiveMixin>().data;
        GenericCreatureHit = lm.damageEffect;
        ElectrocutedEffect = lm.electricalDamageEffect;
        RespawnerPrefab = peeper.GetComponent<CreatureDeath>().respawnerPrefab;

        Done = true;
    }

    public static GameObject GenericCreatureHit { get; private set; }
    public static GameObject ElectrocutedEffect { get; private set; }
    public static GameObject RespawnerPrefab { get; private set; }
}
