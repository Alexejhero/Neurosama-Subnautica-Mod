using SCHIZO.Commands.Attributes;

namespace SCHIZO.SwarmControl.Redeems.Spawns;

#nullable enable
[CommandCategory("Spawns")]
[Redeem(Name = "spawn_aggressive",
    DisplayName = "Spawn Aggressive Creature",
    Description = "Spawn an aggressive creature near the player",
    Announce = AnnounceType.DefaultAnnounce
)]
internal class SpawnAggressiveCreature : SpawnFiltered<SpawnAggressiveCreature.AggressiveCreatures>
{
    public enum AggressiveCreatures
    {
        Ermshark,
        LilyPaddler,
        Cryptosuchus,
        SquidShark,
    }

    protected override float SpawnDistance => 10;
}
