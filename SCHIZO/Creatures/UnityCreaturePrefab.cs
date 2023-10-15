using System;
using System.Diagnostics.CodeAnalysis;
using ECCLibrary;
using Nautilus.Handlers;
using SCHIZO.Items;
using UnityEngine;

namespace SCHIZO.Creatures;

public class UnityCreaturePrefab : UnityPrefab
{
    [SetsRequiredMembers]
    public UnityCreaturePrefab(ModItem item) : base(item)
    {
        if (item.ItemData is not CreatureData) throw new ArgumentException("Item data is not a creature data", nameof(ModItem));
    }

    protected new CreatureData UnityData => (CreatureData) base.UnityData;

    public override void Register()
    {
        base.Register();

        CreatureDataUtils.SetBehaviorType(ModItem, UnityData.BehaviourType);

        if (UnityData.acidImmune) CreatureDataUtils.SetAcidImmune(ModItem);
        if (UnityData.bioReactorCharge > 0) CreatureDataUtils.SetBioreactorCharge(ModItem, UnityData.bioReactorCharge);

        if (UnityData.DatabankInfo)
        {
            PDAHandler.AddCustomScannerEntry(ModItem, UnityData.DatabankInfo.scanTime, encyclopediaKey: PrefabInfo.ClassID);
        }

        // TODO: loot distribution data
    }

    protected override void SetupComponents(GameObject instance)
    {
        base.SetupComponents(instance);

        CreatureDeath creatureDeath = instance.GetComponent<CreatureDeath>();
        if (creatureDeath)
        {
            creatureDeath.respawnerPrefab = ObjectReferences.respawnerPrefab;
        }

        SoundOnDamage soundOnDamage = instance.GetComponent<SoundOnDamage>();
        if (soundOnDamage)
        {
            if (soundOnDamage.damageType == DamageType.Collide) soundOnDamage.sound = ECCSoundAssets.FishSplat;
            else LOGGER.LogWarning($"Creature {PrefabInfo.ClassID} has SoundOnDamage component with damage type {soundOnDamage.damageType} which is not supported");
        }

        LiveMixin liveMixin = instance.GetComponent<LiveMixin>();
        if (liveMixin && liveMixin.data)
        {
            liveMixin.data.damageEffect = ObjectReferences.genericCreatureHit;
            liveMixin.data.deathEffect = ObjectReferences.genericCreatureHit;
            liveMixin.data.electricalDamageEffect = ObjectReferences.electrocutedEffect;
        }
    }
}
