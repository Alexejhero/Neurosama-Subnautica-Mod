using SCHIZO.Commands.Attributes;

namespace SCHIZO.SwarmControl.Redeems.Spawns;

[Redeem(
    Name = "spawn_resource_common",
    DisplayName = "Spawn Common Resource",
    Announce = AnnounceType.DefaultAnnounce
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
