using SCHIZO.Commands.Attributes;

namespace SCHIZO.SwarmControl.Redeems.Spawns;

[CommandCategory("Spawns")]
[Redeem(
    Name = "spawn_resource_common",
    DisplayName = "Spawn Common Resource"
)]
internal class SpawnCommonResource : SpawnFiltered<SpawnCommonResource.CommonResources>
{
    protected override string SpawnThingName => "Resource";
    public enum CommonResources
    {
        Titanium,
        Copper,
        Silver,
        Lead,
    }
}
