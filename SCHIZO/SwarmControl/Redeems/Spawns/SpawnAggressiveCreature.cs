using SCHIZO.Commands.Attributes;
using SCHIZO.SwarmControl.Redeems.Enums;

namespace SCHIZO.SwarmControl.Redeems.Spawns;

#nullable enable
[CommandCategory("Spawns")]
[Redeem(
    Name = "spawn_aggressive",
    DisplayName = "Spawn Aggressive Creature",
    Description = "Spawn an aggressive creature near the player. NOTE: Spawns are automatically cleaned up after some time."
)]
internal class SpawnAggressiveCreature : SpawnFiltered<AggressiveCreature>
{
    protected override float SpawnDistance => 10;
    protected override float Lifetime => 90f;
}
