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
        [TechType.SquidShark] = new("an aggressive shark-squid hybrid"),
        [TechType.IceWorm] = new("a gigantic worm burrowing out of the ground", false),
        [TechType.LilyPaddler] = new("an invisible, imperceptible enemy. The danger level is off the charts"),
        [TechType.Chelicerate] = new("an extremely hostile leviathan-class creature"),
        [TechType.ShadowLeviathan] = new("a massive, aggressive leviathan"),
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
