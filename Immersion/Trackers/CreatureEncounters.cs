using Immersion.Formatting;
using Story;

namespace Immersion.Trackers;

public sealed partial class CreatureEncounters : Tracker
{
    /// <param name="unscannedDescription">A short description of the creature from the perspective of a first encounter.</param>
    /// <param name="firstTimeOnly">Notify only on first encounter and ignore subsequent ones.</param>
    public sealed record EncounterData(string unscannedDescription, bool firstTimeOnly = true);

    public static readonly Dictionary<TechType, EncounterData> Database = new()
    {
        [TechType.SpikeyTrap] = new("some sort of carnivorous plant tentacle"),
        [TechType.SquidShark] = new("an unknown shark-squid hybrid"),
        [TechType.IceWorm] = new("a gigantic worm burrowing out of the ground", false),
        [TechType.LilyPaddler] = new("[PH] nothing. everything. there's no one around. no one is around to help."),
        [TechType.Chelicerate] = new("a massive, extremely hostile leviathan-class creature"),
    };

    public void NotifyCreatureEncounter(TechType creatureTechType)
    {
        if (!Database.TryGetValue(creatureTechType, out EncounterData data)) return;
        if (data.firstTimeOnly && !StoryGoalManager.main.OnGoalComplete(GoalFor(creatureTechType)))
            return;

        string creatureName = PDAScanner.complete.Contains(creatureTechType)
            ? Format.WithArticle(Language.main.Get(creatureTechType))
            : data.unscannedDescription;

        React(Priority.High, Format.FormatPlayer($"{{player}} is being attacked by {creatureName}!"));
    }

    private static string GoalFor(TechType techType) => $"{typeof(CreatureEncounters).FullName}.{techType}";
}
