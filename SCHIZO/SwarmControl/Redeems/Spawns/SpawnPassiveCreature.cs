using SCHIZO.Commands.Attributes;

namespace SCHIZO.SwarmControl.Redeems.Spawns;

#nullable enable
[CommandCategory("Spawns")]
[Redeem(
    Name = "spawn_passive",
    DisplayName = "Spawn Passive Creature",
    Description = "Spawn a passive creature near the player"
)]
internal class SpawnPassiveCreature : SpawnFiltered<SpawnPassiveCreature.PassiveCreature>
{
    public enum PassiveCreature
    {
        Ermfish,
        Anneel,
        Tutel,
        ArcticPeeper,
        Bladderfish,
        Boomerang,
        SpinnerFish,
    }
}
