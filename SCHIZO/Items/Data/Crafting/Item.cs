namespace SCHIZO.Items.Data.Crafting;

partial class Item
{
    private TechType _techType;
    private string _classId;

    public TechType GetTechType()
    {
        if (_techType != TechType.None) return _techType;

        if (!isCustom)
        {
            return _techType = (TechType) techType;
        }
        else
        {
            if (!itemData) return _techType = TechType.None;
            return _techType = itemData.ModItem;
        }
    }

    public string GetClassID()
    {
        if (!string.IsNullOrWhiteSpace(_classId)) return _classId;

        if (!isCustom)
        {
            if (GetTechType() == TechType.None) return _classId = string.Empty;
            return _classId = CraftData.GetClassIdForTechType(GetTechType());
        }
        else
        {
            if (!itemData) return _classId = string.Empty;
            return _classId = itemData.classId;
        }
    }
}
