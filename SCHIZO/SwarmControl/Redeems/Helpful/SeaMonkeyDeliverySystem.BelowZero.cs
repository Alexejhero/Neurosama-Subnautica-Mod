using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Output;
using SCHIZO.Helpers;
using SCHIZO.SwarmControl.Redeems.Enums;
using UnityEngine;
using UWE;
using Random = UnityEngine.Random;

namespace SCHIZO.SwarmControl.Redeems.Helpful;

#nullable enable
[Redeem(
    Name = "redeem_seamonkey_common",
    DisplayName = "Sea Monkey Delivery System",
    Description = "DISCLAIMER: Delivery not guaranteed."
)]
internal class SeaMonkeyDeliverySystem : Command, IParameters
{
    public IReadOnlyList<Parameter> Parameters => [
        new Parameter(new("item", "Item", "Item you wish delivered."), typeof(CommonItems))
    ];

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        if (!Player.main) return CommonResults.Error("Requires a loaded game");

        TechType item;
        try
        {
            if (!ctx.Input.GetNamedArguments().TryGetValue("item", out CommonItems itemFiltered))
                return CommonResults.Error("Invalid item");
            item = (TechType) Enum.Parse(typeof(TechType), itemFiltered.ToString());
        }
        catch (ArgumentException)
        {
            return CommonResults.Error("Invalid item");
        }

        CoroutineHost.StartCoroutine(DeliverCoro(item));

        return "Please note, the streamer may ignore the delivery, or the Sea Monkey may not care enough to deliver. SCHIZOCorp assumes no liability.";
    }

    private static readonly Dictionary<TechType, GameObject> _itemPrefabCache = [];

    private IEnumerator DeliverCoro(TechType item)
    {
        SeaMonkey monke = FindMonke();
        if (!monke)
        {
            TaskResult<SeaMonkey> spawn = new();
            yield return SpawnMonke(spawn);
            monke = spawn.Get();
        }
        LOGGER.LogDebug("(SMDS) Got target");

        if (!_itemPrefabCache.TryGetValue(item, out GameObject itemPrefab))
        {
            IPrefabRequest itemPrefabReq = PrefabDatabase.GetPrefabAsync(CraftData.GetClassIdForTechType(item));
            yield return itemPrefabReq;
            if (!itemPrefabReq.TryGetPrefab(out itemPrefab))
            {
                LOGGER.LogError($"Could not get prefab for {item}"); // should not happen
                yield break;
            }
            LOGGER.LogDebug("(SMDS) Got prefab");
        }

        SeaMonkeyBringGift gift = monke.GetComponent<SeaMonkeyBringGift>();
        gift.maxRange = 9999f;
        gift.evaluatePriority = 999f;
        GameObject itemInstance = GameObject.Instantiate(itemPrefab);
        gift.heldItem.Hold(itemInstance);

        gift.timeLastGiftAnimation = -9999f;
        SeaMonkeyBringGift.timeLastGlobalSeaMonkeyGift = -9999f;

        if (monke.prevBestAction)
            monke.prevBestAction.StopPerform(Time.time);
        if (gift.Evaluate(Time.time) == 0)
        {
            LOGGER.LogWarning("Sea Monkey got distracted, unlucky");
        }
        gift.swimVelocity *= 1.5f; // express delivery
        monke.TryStartAction(gift);

        LOGGER.LogInfo("(SMDS) Delivering");
    }

    private static SeaMonkey FindMonke()
    {
        return PhysicsHelpers.ObjectsInRange(Player.main.transform.position, 100)
            .OfTechType(TechType.SeaMonkey)
            .SelectComponentInParent<SeaMonkey>() // sphere cast hits the collider which is on the model
            .FirstOrDefault(m =>
            {
                SeaMonkeyHeldItem held = m.GetComponentInChildren<SeaMonkeyHeldItem>();
                return held && !held.IsHoldingItem();
            });
    }

    private static GameObject? _monkePrefab;
    private IEnumerator SpawnMonke(IOut<SeaMonkey> result)
    {
        if (!_monkePrefab)
        {
            IPrefabRequest request = PrefabDatabase.GetPrefabAsync(CraftData.GetClassIdForTechType(TechType.SeaMonkey));
            yield return request;
            request.TryGetPrefab(out _monkePrefab);
        }
        yield return new WaitUntil(() => SwarmControlManager.Instance.CanSpawn);

        Player player = Player.main;

        Vector3 deltaBehind = 10f * (Random.onUnitSphere - player.transform.forward);
        Vector3 spawnPos = player.transform.position + deltaBehind;

        // basic "spawning inside a wall" check
        int max = UWE.Utils.RaycastIntoSharedBuffer(player.transform.position, spawnPos);
        // closest blocking ray hit
        RaycastHit blockingHit = UWE.Utils.sharedHitBuffer.Take(max)
            .Where(hit => hit.collider.gameObject != player.gameObject)
            .OrderBy(hit => hit.point.DistanceSqrXZ(player.transform.position))
            .FirstOrDefault();
        if (blockingHit.point != default)
            spawnPos = blockingHit.point + blockingHit.normal * 0.1f;

        GameObject instance = GameObject.Instantiate(_monkePrefab!);
        instance.transform.position = spawnPos;
        UnityEngine.Object.Destroy(instance.GetComponent<LargeWorldEntity>());
        result.Set(instance.GetComponentInChildren<SeaMonkey>());
    }
}
