using SCHIZO.Commands.Attributes;

namespace SCHIZO.SwarmControl.Redeems.Helpful;

[CommandCategory("Spawns")]
[Redeem(
    Name = "give_common",
    DisplayName = "Give Common Resource"
)]
internal class GiveCommonResource : ItemFiltered<GiveCommonResource.CommonResources>
{
    protected override string SpawnThingName => "Item";
    public enum CommonResources
    {
        Salt,
        Quartz,
        Titanium,
        Copper,
        Silver,
        Lead,
        RibbonPlant,
    }
}
