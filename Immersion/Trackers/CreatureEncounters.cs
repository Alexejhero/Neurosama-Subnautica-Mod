using Immersion.Formatting;
using Story;

namespace Immersion.Trackers;

public sealed class CreatureEncounters : Tracker
{
    /// <param name="UnscannedDescription">A short description of the creature from the perspective of a first encounter.</param>
    /// <param name="FirstTimeOnly">Notify only on first encounter and ignore subsequent ones.</param>
    public sealed record EncounterData(string UnscannedDescription, bool FirstTimeOnly = true, float Cooldown = 60f)
    {
        private float _nextTime;
        public bool CanTrigger => Time.time > _nextTime;
        public void NotifyTriggered() => _nextTime = Time.time + Cooldown;
    }

    private enum TargetType
    {
        Player,
        Vehicle,
    }

    private static readonly Dictionary<TechType, EncounterData> PlayerAttacks = new()
    {
        [TechType.SpikeyTrap] = new("Some sort of carnivorous plant tentacle has grabbed {player} and is pulling {object} in!"),
        [TechType.IceWorm] = new("A gigantic worm is burrowing out of the frozen ground!", false),
        [TechType.LilyPaddler] = new("{player} is hallucinating on some hardcore drugs."),
        [TechType.SnowStalker] = new("{player} is pinned down by a huge bear-like beast!"),
        [TechType.SquidShark] = new("{player} is struggling against the jaws of a shark-squid hybrid!"),
        // player does not survive the attack below this line
        [TechType.Chelicerate] = new("An extremely hostile beaked leviathan has grabbed {player} and is about to consume {object} whole!"),
        [TechType.ShadowLeviathan] = new("A massive, deadly leviathan has {player} in its grasp. There is no hope of escape."),
    };

    private static readonly Dictionary<TechType, EncounterData> VehicleAttacks = new()
    {
        // only leviathan attacks grab vehicles
        [TechType.Chelicerate] = new("An aggressive, beaked leviathan has {player}'s {vehicle} in its mandibles!", false),
        [TechType.ShadowLeviathan] = new("A massive, deadly leviathan has {player}'s {vehicle} in its grasp and is about to slobber acid all over it!", false),
        // todo: messages for when the vehicle gets released might be good too?
    };

    public void NotifyCreatureEncounter(TechType creatureTechType, MonoBehaviour target)
    {
        TargetType targetType = target switch
        {
            Player => TargetType.Player,
            SeaTruckSegment or Exosuit => TargetType.Vehicle,
            _ => throw new InvalidOperationException()
        };
        Dictionary<TechType, EncounterData> database = targetType == TargetType.Player
            ? PlayerAttacks
            : VehicleAttacks;
        if (!database.TryGetValue(creatureTechType, out EncounterData data)) return;
        if (data.FirstTimeOnly && !StoryGoalManager.main.OnGoalComplete(GoalFor(creatureTechType, targetType)))
            return;
        if (!data.CanTrigger) return;

        string message;
        if (PDAScanner.complete.Contains(creatureTechType))
        {
            string creatureName = Format.WithArticle(Language.main.Get(creatureTechType));
            message = $"{{player}}{(targetType == TargetType.Player ? "" : "'s {vehicle}")} is being attacked by {creatureName}!";
        }
        else
        {
            message = data.UnscannedDescription;
        }
        data.NotifyTriggered();
        if (targetType == TargetType.Vehicle)
        {
            string vehicle = target is SeaTruckSegment ? "seatruck" : "prawn suit";
            message = message.Replace("{vehicle}", vehicle);
        }

        React(Priority.High, Format.FormatPlayer(message));
    }

    private static string GoalFor(TechType techType, TargetType targetType) => $"{typeof(CreatureEncounters).FullName}.{techType}.{targetType}";
}
