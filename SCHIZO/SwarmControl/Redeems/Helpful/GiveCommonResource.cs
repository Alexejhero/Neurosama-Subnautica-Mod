using SCHIZO.Commands.Attributes;
using SCHIZO.SwarmControl.Redeems.Enums;

namespace SCHIZO.SwarmControl.Redeems.Helpful;

[CommandCategory("Spawns")]
[Redeem(
    Name = "give_common",
    DisplayName = "Give Common Resource",
    Export = false /// deprecated in favor of <see cref="SeaMonkeyDeliverySystem"/>
)]
internal class GiveCommonResource : ItemFiltered<CommonResources>
{
    protected override string SpawnThingName => "Item";
}
