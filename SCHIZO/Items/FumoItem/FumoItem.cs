using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets.Gadgets;
using Nautilus.Handlers;
using SCHIZO.Extensions;
using UnityEngine;

namespace SCHIZO.Items.FumoItem;

public sealed class FumoItem : UnityPrefab
{
    [SetsRequiredMembers]
    public FumoItem(ModItem modItem) : base(modItem)
    {
    }

    public override void Register()
    {
        this.SetEquipment(EquipmentType.Hand)
            .WithQuickSlotType(QuickSlotType.Selectable);

        base.Register();
    }

    protected override void ModifyPrefab(GameObject prefab)
    {
        base.ModifyPrefab(prefab);
        prefab.EnsureComponentFields();
    }

    protected override void PostRegister()
    {
#if BELOWZERO
        CraftDataHandler.SetColdResistance(modItem, 20);
#endif
        base.PostRegister();
    }
}
