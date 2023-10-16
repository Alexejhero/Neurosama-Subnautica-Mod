namespace SCHIZO.Items.Data.Crafting;

partial class Item
{
    private TechType _converted;

    public TechType Convert()
    {
        if (_converted != TechType.None) return _converted;

        return _converted = !isCustom ? (TechType) techType : itemData.ModItem;
    }
}
