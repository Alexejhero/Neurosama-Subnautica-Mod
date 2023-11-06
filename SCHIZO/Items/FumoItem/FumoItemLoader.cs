namespace SCHIZO.Items.FumoItem;

partial class FumoItemLoader
{
    public override void Load()
    {
        new FumoItem(itemData.ModItem).Register();
    }
}
