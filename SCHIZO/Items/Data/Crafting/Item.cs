namespace SCHIZO.Items.Data.Crafting;

partial class Item
{
    public TechType Convert()
    {
        if (!isCustom) return (TechType) techType;
        else return itemData.ModItem;
    }
}
