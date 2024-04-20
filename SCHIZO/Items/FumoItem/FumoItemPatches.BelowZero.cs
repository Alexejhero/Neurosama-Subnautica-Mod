using System.Collections;
using HarmonyLib;
using SCHIZO.Spawns;
using UnityEngine;
using UWE;

namespace SCHIZO.Items.FumoItem;
[HarmonyPatch]
partial class FumoItemPatches
{
    private static bool _registered;
    private static ModItem fumoItem;
    private static SpawnLocation spawnLoc; // local to lifepod

    public static void Register(ModItem modItem, SpawnLocation loc)
    {
        if (_registered) LOGGER.LogWarning("Fumo item spawns already registered - overriding previous");
        _registered = true;

        fumoItem = modItem;
        spawnLoc = loc;
    }

    [HarmonyPatch(typeof(LifepodDrop), nameof(LifepodDrop.OnSettled))]
    [HarmonyPostfix]
    public static void SpawnFumoInLifepodDrop(LifepodDrop __instance)
    {
        if (!_registered) return;

        CoroutineHost.StartCoroutine(SpawnFumoCoro(__instance));
    }
    // code "adapted" from nautilus's EntitySpawner
    private static IEnumerator SpawnFumoCoro(LifepodDrop pod)
    {
        string fumoItemClassId = fumoItem.PrefabInfo.ClassID;

        IPrefabRequest request = PrefabDatabase.GetPrefabAsync(fumoItemClassId);
        yield return request;
        if (!request.TryGetPrefab(out GameObject prefab))
        {
            LOGGER.LogWarning("Could not get prefab for fumo item, will not spawn in lifepod - unlocking automatically");
            KnownTech.Add(fumoItem, false, false);
        }

        // shouldn't spawn twice anymore w/ new code but shrug
        if (Object.FindObjectOfType<FumoItemTool>())
        {
            LOGGER.LogWarning("Attempted to spawn fumo in lifepod twice");
            yield break;
        }
        yield return new WaitUntil(() => LargeWorldStreamer.main && LargeWorldStreamer.main.IsReady());
        LargeWorldStreamer lws = LargeWorldStreamer.main;

        Int3 batch = lws.GetContainingBatch(pod.transform.localToWorldMatrix.MultiplyPoint(spawnLoc.position));
        yield return new WaitUntil(() => lws.IsBatchReadyToCompile(batch));

        yield return new WaitUntil(() => LargeWorld.main && LargeWorld.main.streamer.globalRoot != null);

        LOGGER.LogMessage("Spawning fumo in lifepod drop");
        GameObject obj = UWE.Utils.InstantiateDeactivated(prefab, pod.transform, spawnLoc.position, Quaternion.Euler(spawnLoc.rotation));
        obj.SetActive(true);

        LargeWorldEntity.Register(obj);
    }
}
