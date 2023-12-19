namespace SCHIZO.Items.FumoItem;

partial class FumoItemLoader
{
    public override void Load(ModItem modItem)
    {
        base.Load(modItem);
        FumoItemPatches.Register(modItem, spawn);
    }
}
