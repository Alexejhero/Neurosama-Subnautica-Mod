using Immersion.Formatting;
using Story;

namespace Immersion.Trackers;

public sealed partial class CreatureEncounters : Tracker
{
    /// <param name="unscannedDescription">A short description of the creature from the perspective of a first encounter.</param>
    /// <param name="firstTimeOnly">Notify only on first encounter and ignore subsequent ones.</param>
    public sealed record EncounterData(string UnscannedDescription, bool FirstTimeOnly = true, float Cooldown = 60f)
    {
        private float _nextTime;
        public bool CanTrigger => Time.time > _nextTime;
        public void NotifyTriggered() => _nextTime = Time.time + Cooldown;
    }

    public static readonly Dictionary<TechType, EncounterData> Database = new()
    {
        [TechType.SpikeyTrap] = new("Some sort of carnivorous plant tentacle has grabbed {player} and is pulling {object} in!"),
        [TechType.IceWorm] = new("A gigantic worm is burrowing out of the frozen ground!", false),
        [TechType.LilyPaddler] = new("{player} is hallucinating on drugs. Respond in caveman speech, using only iambic pentameter."),
        [TechType.SnowStalker] = new("{player} is pinned down by a huge bear-like beast!"),
        [TechType.SquidShark] = new("{player} is struggling against the jaws of a shark-squid hybrid!"),
        [TechType.Chelicerate] = new("An extremely hostile leviathan-class creature has grabbed {player} and is about to consume {object} whole!"),
        [TechType.ShadowLeviathan] = new("A massive, deadly leviathan has {player} in its grasp. There is no hope of escape."),
    };

    public void NotifyCreatureEncounter(TechType creatureTechType)
    {
        if (!Database.TryGetValue(creatureTechType, out EncounterData data)) return;
        if (data.FirstTimeOnly && !StoryGoalManager.main.OnGoalComplete(GoalFor(creatureTechType)))
            return;
        if (!data.CanTrigger) return;

        string message;
        if (PDAScanner.complete.Contains(creatureTechType))
        {
            string creatureName = Format.WithArticle(Language.main.Get(creatureTechType));
            message = $"{{player}} is being attacked by {creatureName}!";
        }
        else
        {
            message = data.UnscannedDescription;
        }
        data.NotifyTriggered();

        React(Priority.High, Format.FormatPlayer(message));
    }

    private static string GoalFor(TechType techType) => $"{typeof(CreatureEncounters).FullName}.{techType}";
}
