using SCHIZO.Commands.Attributes;
using SCHIZO.SwarmControl.Redeems.Enums;

namespace SCHIZO.SwarmControl.Redeems.Spawns;

#nullable enable
[CommandCategory("Spawns")]
[Redeem(
    Name = "spawn_passive",
    DisplayName = "Spawn Passive Creature",
    Description = "Spawn a passive creature near the player"
)]
internal class SpawnPassiveCreature : SpawnFiltered<PassiveCreature>;
