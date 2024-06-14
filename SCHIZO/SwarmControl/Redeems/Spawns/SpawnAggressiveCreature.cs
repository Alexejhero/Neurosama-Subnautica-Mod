using SCHIZO.Commands.Attributes;
using SCHIZO.SwarmControl.Redeems.Enums;

namespace SCHIZO.SwarmControl.Redeems.Spawns;

#nullable enable
[CommandCategory("Spawns")]
[Redeem(Name = "spawn_aggressive",
    DisplayName = "Spawn Aggressive Creature",
    Description = "Spawn an aggressive creature near the player"
)]
internal class SpawnAggressiveCreature : SpawnFiltered<AggressiveCreature>
{
    protected override float SpawnDistance => 10;
}
