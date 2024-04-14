using System.Collections;
using SCHIZO.Helpers;
using UnityEngine;
using UWE;

namespace SCHIZO.Items.FumoItem;

partial class SneakyFumoSpawner
{
    // prevent dupes or whatever
    private const string _storyGoal = "SneakyFumo";
    private GameObject _prefab;

    private IEnumerator Start()
    {
        if (StoryGoalHelpers.IsCompleted(_storyGoal))
            yield return TheFumoAppears();
        else
            yield return NeuroFumoIs100mFromYourLocationAndRapidlyApproaching();
    }

    private IEnumerator NeuroFumoIs100mFromYourLocationAndRapidlyApproaching()
    {
        LOGGER.LogWarning("starting");
        // player enters drop pod for the first time, no fumo is present
        while (!StoryGoalHelpers.IsCompleted("OnEnterLifepod"))
            yield return new WaitForSecondsRealtime(5);
        LOGGER.LogWarning("entered");

        // player leaves for a while
        while (!StoryGoalHelpers.IsCompleted("OnExitLifepod"))
            yield return new WaitForSecondsRealtime(5);
        LOGGER.LogWarning("exited");

        yield return new WaitForSeconds(minAwayTime); // probably completely useless
        LOGGER.LogWarning("waited");
        while ((Player.main.transform.position - transform.position).magnitude < minAwayDistance)
            yield return new WaitForSecondsRealtime(1);
        LOGGER.LogWarning("away");

        yield return TheFumoAppears();
    }

    private IEnumerator GetPrefab()
    {
        IPrefabRequest request = PrefabDatabase.GetPrefabAsync(spawnData.classId);
        yield return request;
        if (!request.TryGetPrefab(out _prefab))
        {
            LOGGER.LogError("Could not get prefab for fumo item, will not spawn in lifepod");
            // if there's no prefab, unlocking it is probably not a good idea
            // KnownTech.Add(_fumoItem, false, false);
            Destroy(this);
            yield break;
        }
    }

    private IEnumerator TheFumoAppears()
    {
        StoryGoalHelpers.Trigger(_storyGoal);

        yield return GetPrefab();
        if (!_prefab) yield break;

        GameObject fumo = GameObject.Instantiate(_prefab, transform);
        fumo.transform.parent = transform.parent; // reparent to drop pod (keeping the spawner's local pos/rot)
        LargeWorldEntity.Register(fumo);

        Destroy(this);
    }
}
