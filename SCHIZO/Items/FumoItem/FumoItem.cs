using System.Diagnostics.CodeAnalysis;
using Nautilus.Handlers;

namespace SCHIZO.Items.FumoItem;

public sealed class FumoItem : UnityPrefab
{
    [SetsRequiredMembers]
    public FumoItem(ModItem modItem) : base(modItem)
    {
    }

    public override void Register()
    {
        CraftDataHandler.SetEquipmentType(ModItem, EquipmentType.Hand);
        CraftDataHandler.SetQuickSlotType(ModItem, QuickSlotType.Selectable);

        base.Register();
    }

    protected override void PostRegister()
    {
#if BELOWZERO
        Nautilus.Handlers.CraftDataHandler.SetColdResistance(ModItem, 20);
#endif
        base.PostRegister();
    }
}
