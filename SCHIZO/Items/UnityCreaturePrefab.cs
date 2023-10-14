using System;
using System.Diagnostics.CodeAnalysis;
using ECCLibrary;
using Nautilus.Handlers;

namespace SCHIZO.Items;

public class UnityCreaturePrefab : UnityPrefab
{
    [SetsRequiredMembers]
    protected UnityCreaturePrefab(ModItem item) : base(item)
    {
        if (item.ItemData is not Unity.Creatures.CreatureData cd) throw new ArgumentException("Item data is not a creature data", nameof(ModItem));
    }

    protected new Unity.Creatures.CreatureData UnityData => (Unity.Creatures.CreatureData) base.UnityData;

    public new static void CreateAndRegister(ModItem modItem)
    {
        new UnityCreaturePrefab(modItem).Register();
    }

    public override void Register()
    {
        base.Register();
        if (UnityData.DatabankInfo)
        {
            PDAHandler.AddCustomScannerEntry(ModItem, UnityData.DatabankInfo.scanTime, encyclopediaKey: PrefabInfo.ClassID);
        }

        if (UnityData.acidImmune) CreatureDataUtils.SetAcidImmune(ModItem);
        if (UnityData.bioReactorCharge > 0) CreatureDataUtils.SetBioreactorCharge(ModItem, UnityData.bioReactorCharge);

        // TODO: creature sounds

        // TODO: loot distribution data
    }
}
