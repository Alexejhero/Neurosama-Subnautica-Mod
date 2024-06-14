using System.Runtime.Serialization;

namespace SCHIZO.SwarmControl.Redeems;

public enum CommonTechTypes
{
    // modded creatures/items
    Ermfish,
    Ermshark,
    Tutel,
    Anneel,
    [EnumMember(Value = "Neuro fumo")]
    NeuroFumoItem,
    [EnumMember(Value = "Evil fumo")]
    EvilFumoItem,

    // resources
    Salt,
    Quartz,
    Titanium,
    Copper,
    Silver,
    Lead,
    Gold,
    [EnumMember(Value = "Scrap Metal")]
    ScrapMetal,
    Lithium,
    Magnetite,
    Kyanite,
    Nickel,
    Diamond,

    // creatures
    [EnumMember(Value = "Arctic Peeper")]
    ArcticPeeper,
    Bladderfish,

    // story/rare stuff
    RadioTowerPPUFragment,
}
