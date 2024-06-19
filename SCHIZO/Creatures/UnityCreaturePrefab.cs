using System;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Handlers;
using SCHIZO.Helpers;
using SCHIZO.Items;
using UnityEngine;
using OurCreatureData = SCHIZO.Creatures.Data.CreatureData;

namespace SCHIZO.Creatures;

public class UnityCreaturePrefab : UnityPrefab
{
    [SetsRequiredMembers]
    public UnityCreaturePrefab(ModItem item) : base(item)
    {
        if (item.ItemData is not OurCreatureData) throw new ArgumentException("Item data is not a creature data", nameof(ModItem));
    }

    protected new OurCreatureData UnityData => (OurCreatureData) base.UnityData;

    protected override void SetItemProperties()
    {
        base.SetItemProperties();

        CreatureData.behaviourTypeList[ModItem] = UnityData.BehaviourType;

        if (UnityData.acidImmune) DamageSystem.acidImmune = [.. DamageSystem.acidImmune, ModItem];
        if (UnityData.bioReactorCharge > 0) BaseBioReactor.charge[ModItem] = UnityData.bioReactorCharge;

        if (UnityData.isPickupable)
        {
            CraftDataHandler.SetQuickSlotType(ModItem, QuickSlotType.Selectable);
            CraftDataHandler.SetEquipmentType(ModItem, EquipmentType.Hand);
        }
    }

    protected override void SetupComponents(GameObject instance)
    {
        base.SetupComponents(instance);
        if (!ObjectReferences.Done) LOGGER.LogFatal("Object references haven't loaded yet! Wait until ObjectReferences.Done");

        CreatureDeath creatureDeath = instance.GetComponent<CreatureDeath>();
        if (creatureDeath)
        {
            creatureDeath.respawnerPrefab = ObjectReferences.RespawnerPrefab;
        }

        SoundOnDamage soundOnDamage = instance.GetComponent<SoundOnDamage>();
        if (soundOnDamage)
        {
            if (soundOnDamage.damageType == DamageType.Collide) soundOnDamage.sound = FMODHelpers.GameEvents.FishSplat;
            else LOGGER.LogWarning($"Creature {PrefabInfo.ClassID} has SoundOnDamage component with damage type {soundOnDamage.damageType} which is not supported");
        }

        LiveMixin liveMixin = instance.GetComponent<LiveMixin>();
        if (liveMixin && liveMixin.data)
        {
            liveMixin.data.damageEffect = ObjectReferences.GenericCreatureHit;
            liveMixin.data.deathEffect = ObjectReferences.GenericCreatureHit;
            liveMixin.data.electricalDamageEffect = ObjectReferences.ElectrocutedEffect;
        }

        if (UnityData.waterParkData)
        {
            WaterParkCreature wpc = instance.EnsureComponent<WaterParkCreature>();
            wpc.data = UnityData.waterParkData.GetData();
        }
    }
}
