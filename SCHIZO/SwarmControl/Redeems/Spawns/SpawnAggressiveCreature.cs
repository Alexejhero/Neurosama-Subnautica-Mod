using SCHIZO.Commands.Attributes;

namespace SCHIZO.SwarmControl.Redeems.Spawns;
#nullable enable
[CommandCategory("Spawns")]
[Command(Name = "spawn_aggressive",
    DisplayName = "Spawn Aggressive Creature",
    Description = "Spawn an aggressive creature near the player")]
internal class SpawnAggressiveCreature : SpawnFiltered<SpawnAggressiveCreature.AggressiveCreatures>
{
    public enum AggressiveCreatures
    {
        Ermshark,
        LilyPaddler,
        Cryptosuchus,
        SquidShark,
    }
}
