namespace SCHIZO.Items.Gymbag;

partial class GymbagLoader
{
    public override void Load()
    {
        new Gymbag(itemData.ModItem, itemData.CloneTarget).Register();
    }
}
