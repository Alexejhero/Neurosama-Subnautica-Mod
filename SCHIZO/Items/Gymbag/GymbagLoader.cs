using SCHIZO.Items.Data;

namespace SCHIZO.Items.Gymbag;

partial class GymbagLoader
{
    public override void Load()
    {
        new Gymbag(itemData.ModItem, ((CloneItemData)itemData).CloneTarget).Register();
    }
}
