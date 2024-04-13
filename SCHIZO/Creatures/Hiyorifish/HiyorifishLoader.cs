using SCHIZO.Items;

namespace SCHIZO.Creatures.Hiyorifish;
partial class HiyorifishLoader
{
    public override void Load(ModItem modItem)
    {
        base.Load(modItem);
        OverrideScanProgress.Register(modItem, OverrideScanProgress.Limit(scanProgressLimit));
    }
}
