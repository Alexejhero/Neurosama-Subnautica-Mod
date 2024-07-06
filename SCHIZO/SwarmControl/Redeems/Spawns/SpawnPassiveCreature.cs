using SCHIZO.Commands.Attributes;
using SCHIZO.SwarmControl.Redeems.Enums;

namespace SCHIZO.SwarmControl.Redeems.Spawns;

#nullable enable
[CommandCategory("Spawns")]
[Redeem(
    Name = "spawn_passive",
    DisplayName = "Spawn Passive Creature",
    Description = "Spawn a passive creature near the player. NOTE: Spawns are automatically cleaned up after some time."
)]
internal class SpawnPassiveCreature : SpawnFiltered<PassiveCreature>
{
    protected override float Lifetime => 300f;
}
