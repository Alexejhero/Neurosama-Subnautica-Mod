using System.Runtime.Serialization;

namespace SCHIZO.SwarmControl.Redeems.Enums;

public enum CommonItems
{
    Salt,
    Quartz,
    Titanium,
    Copper,
    Silver,
    Lead,

    [EnumMember(Value = "Neuro fumo")]
    NeuroFumoItem,
    [EnumMember(Value = "Evil fumo")]
    EvilFumoItem,
    Ermfish,
    Tutel,

    Gold,
    [EnumMember(Value = "Table Coral")]
    JeweledDiskPiece,
    [EnumMember(Value = "Ribbon Plant")]
    GenericRibbon,

    [EnumMember(Value = "Filtered Water")]
    FilteredWater,
    [EnumMember(Value = "Nutrient Block")]
    NutrientBlock
}
