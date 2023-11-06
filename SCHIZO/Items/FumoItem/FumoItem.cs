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
        base.Register();
#if BELOWZERO
        CraftDataHandler.SetColdResistance(ModItem, 20);
#endif
    }
}
