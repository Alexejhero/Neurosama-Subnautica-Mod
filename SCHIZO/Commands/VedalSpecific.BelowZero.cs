using System.Collections;
using System.Collections.Generic;
using SCHIZO.Commands.Attributes;
using UnityEngine;
using UWE;

namespace SCHIZO.Commands;

[CommandCategory("Vedal")]
internal static class VedalSpecific
{
    private static bool _ashleyRespawned;

    [Command(
        Name = "revive_ashley",
        DisplayName = "Revive Ashley",
        Description = "Reimburse Vedal for the Ashley accident",
        RegisterConsoleCommand = true
    )]
    public static void ReimburseAshley()
    {
        CoroutineHost.StartCoroutine(DoInsuranceRoutine());
    }

    private static IEnumerator DoInsuranceRoutine()
    {
        DevCommands.Say("Hello, this is a representative speaking on behalf of SCHIZOCorp.");
        yield return new WaitForSeconds(3);
        DevCommands.Say("We are contacting you today regarding your Prawn Suit's extended warranty.");
        yield return new WaitForSeconds(5);
        DevCommands.Say("Your insurance claim for accident coverage concerning one (1) Prawn Suit (name \"Ashley\") has been reviewed by our experts.");
        yield return new WaitForSeconds(6);
        DevCommands.Say("A thorough investigation has determined that you are entitled for compensation.");
        yield return new WaitForSeconds(4);
        DevCommands.Say("According to the terms set out in your agreement with SCHIZOCorp, Ashley will be restored and delivered to you shortly.");
        yield return new WaitForSeconds(8);

        yield return SpawnAshleyCoro();

        DevCommands.Say(_ashleyRespawned
            ? "Please visit your LifePod™ at your earliest convenience to receive your reimbursement."
            : "Erm, there seems to have been an issue delivering your reimbursement. Please contact our support team for further assistance."
        );
        yield return new WaitForSeconds(4);
        DevCommands.Say("Thank you for being a valued customer of SCHIZOCorp.");
    }

    private static IEnumerator SpawnAshleyCoro()
    {
        CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.Exosuit);
        yield return task;
        if (task.GetResult() is not GameObject prefab || !prefab)
        {
            LOGGER.LogError("no prawn suit prefab bozo");
            yield break;
        }
        GameObject instance = UWE.Utils.InstantiateDeactivated(prefab);
        Vector3 spawnPos = Object.FindObjectOfType<LifepodDrop>().transform.position;
        const float range = 20;
        spawnPos += new Vector3(Random.Range(-range,range), 20, Random.Range(-range,range));
        instance.transform.position = spawnPos;

        Exosuit ashley = instance.GetComponent<Exosuit>();
        ashley.LazyInitialize();

        ICustomizeable nameplate = ashley.colorNameControl;
        nameplate.SetName("Ashley"); // luckily colors were all default

        Dictionary<string, TechType> equipment = new()
        {
            ["ExosuitArmLeft"] = TechType.ExosuitPropulsionArmModule,
            ["ExosuitArmRight"] = TechType.ExosuitDrillArmModule,
            ["ExosuitModule1"] = TechType.ExosuitJetUpgradeModule,
            ["ExosuitModule2"] = TechType.VehicleStorageModule,
            ["ExosuitModule3"] = TechType.ExoHullModule2,
        };

        foreach (KeyValuePair<string, TechType> pair in equipment)
        {
            (string slot, TechType type) = (pair.Key, pair.Value);
            CoroutineTask<GameObject> prefabRequest = CraftData.GetPrefabForTechTypeAsync(pair.Value);
            yield return prefabRequest;
            if (prefabRequest.GetResult() is not GameObject itemPrefab || !itemPrefab)
            {
                LOGGER.LogError($"no {type} bozo");
                continue;
            }
            GameObject itemInstance = UWE.Utils.InstantiateDeactivated(itemPrefab);
            Pickupable p = itemInstance.GetComponent<Pickupable>();
            p.Initialize();
            InventoryItem item = new(p);
            ashley.modules.AddItem(slot, item, true);
        }

        (TechType, int)[] items =
        [
            (TechType.Quartz, 1),
            (TechType.FiberMesh, 2),
            (TechType.Copper, 5),
            (TechType.Silver, 2),
            (TechType.Lithium, 1),
            (TechType.Magnetite, 1),
            (TechType.UraniniteCrystal, 1),
            (TechType.PrecursorIonCrystal, 1),
        ];
        foreach ((TechType type, int amount) in items)
        {
            CoroutineTask<GameObject> prefabRequest = CraftData.GetPrefabForTechTypeAsync(type);
            yield return prefabRequest;
            if (prefabRequest.GetResult() is not GameObject itemPrefab || !itemPrefab)
            {
                LOGGER.LogError($"no {type} bozo");
                continue;
            }
            for (int i = 0; i < amount; i++)
            {
                GameObject itemInstance = UWE.Utils.InstantiateDeactivated(itemPrefab);
                Pickupable p = itemInstance.GetComponent<Pickupable>();
                ashley.storageContainer.container.AddItem(p);
            }
        }

        instance.SetActive(true);
        LargeWorldEntity.Register(instance);
        CrafterLogic.NotifyCraftEnd(instance, TechType.Exosuit);
        instance.SendMessage("StartConstruction", SendMessageOptions.DontRequireReceiver);

        _ashleyRespawned = true;
    }
}
