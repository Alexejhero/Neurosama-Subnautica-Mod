namespace SCHIZO.Items.Data.Crafting;

partial class Item
{
    private TechType? _techType;
    private string _classId;

    public TechType GetTechType()
        => _techType ??= CacheTechType();

    private TechType CacheTechType()
    {
        return isCustom ? (itemData ? itemData.ModItem : default)
            : (TechType) techType;
    }

    public string GetClassID()
        => _classId ??= CacheClassID();

    private string CacheClassID()
    {
        return isCustom ? (itemData ? itemData.classId : null)
            : CraftData.GetClassIdForTechType(GetTechType()); // returns null if not found
    }

    public static bool IsValid(Item item)
        => item.isCustom ? item.itemData
            : item.techType != default;
}
