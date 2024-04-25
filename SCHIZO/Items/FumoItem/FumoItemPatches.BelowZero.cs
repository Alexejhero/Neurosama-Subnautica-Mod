using System.Collections;
using HarmonyLib;
using UnityEngine;
using UWE;

namespace SCHIZO.Items.FumoItem;
[HarmonyPatch]
partial class FumoItemPatches
{
    private static string _spawnerClassId;

    public static void Register(string spawnerClassId)
    {
        if (_spawnerClassId is { })
            LOGGER.LogWarning("Fumo item spawns already registered - overriding previous");
        _spawnerClassId = spawnerClassId;
    }

    [HarmonyPatch(typeof(LifepodDrop), nameof(LifepodDrop.OnSettled))]
    [HarmonyPostfix]
    public static void SpawnFumoInLifepodDrop(LifepodDrop __instance)
    {
        if (_spawnerClassId is null) return;

        CoroutineHost.StartCoroutine(SpawnFumoCoro(__instance));
    }

    // code "adapted" from nautilus's EntitySpawner
    private static IEnumerator SpawnFumoCoro(LifepodDrop pod)
    {
        IPrefabRequest request = PrefabDatabase.GetPrefabAsync(_spawnerClassId);
        yield return request;
        if (!request.TryGetPrefab(out GameObject spawnerPrefab))
        {
            LOGGER.LogError("Could not get prefab for fumo spawner, will not spawn in lifepod");
            yield break;
        }
        yield return new WaitUntil(() => LargeWorldStreamer.main && LargeWorldStreamer.main.IsReady());
        LargeWorldStreamer lws = LargeWorldStreamer.main;

        Int3 batch = lws.GetContainingBatch(pod.transform.TransformPoint(spawnerPrefab.transform.position));
        yield return new WaitUntil(() => lws.IsBatchReadyToCompile(batch));

        yield return new WaitUntil(() => LargeWorld.main && LargeWorld.main.streamer.globalRoot != null);

        LOGGER.LogMessage("Spawning fumo spawner in lifepod drop");
        GameObject obj = GameObject.Instantiate(spawnerPrefab, pod.transform, false);
        LargeWorldEntity lwe = obj.EnsureComponent<LargeWorldEntity>();
        lwe.cellLevel = LargeWorldEntity.CellLevel.Global; // do not unload under any circumstances

        LargeWorldEntity.Register(obj);
    }
}
