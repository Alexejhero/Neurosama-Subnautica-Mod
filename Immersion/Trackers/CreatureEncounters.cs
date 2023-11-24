using Story;

namespace Immersion.Trackers;

public sealed class CreatureEncounters : Tracker
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="unscannedDescription">A short description of the creature from the perspective of a first encounter.</param>
    /// <param name="scannedName">
    /// The name of the creature. Should be preceded by the correct indefinite article
    /// (it's <see href="https://stackoverflow.com/questions/2585059/library-to-determine-indefinite-article-of-a-noun">a pain in the ass</see> to do programmatically)
    /// </param>
    /// <param name="firstTimeOnly"></param>
    public sealed record EncounterData(string unscannedDescription, string scannedName, bool firstTimeOnly = true);

    public static readonly Dictionary<TechType, EncounterData> Database = new()
    {
        [TechType.SpikeyTrap] = new("some sort of carnivorous plant tentacle", "a Spikey Trap"),
        [TechType.SquidShark] = new("an unknown shark-squid hybrid", "a Squid Shark"),
        [TechType.IceWorm] = new("a gigantic worm burrowing out of the ground", "an Ice Worm"),
        [TechType.LilyPaddler] = new("[PH] nothing. everything. there's no one around. no one is around to help.", "a Lily Paddler"),
        [TechType.Chelicerate] = new("a massive, extremely hostile leviathan-class creature", "a Chelicerate"),
    };
    
    public void NotifyCreatureEncounter(TechType creatureTechType)
    {
        if (!Database.TryGetValue(creatureTechType, out EncounterData data)) return;
        if (data.firstTimeOnly && !StoryGoalManager.main.OnGoalComplete(GoalFor(creatureTechType)))
            return;
        bool scanned = PDAScanner.complete.Contains(creatureTechType);
        string creatureName = scanned ? data.scannedName : data.unscannedDescription;
        React(Priority.High, $"{Globals.PlayerName} is being attacked by {creatureName}!");
    }

    private static string GoalFor(TechType techType) => $"{typeof(CreatureEncounters).FullName}.{techType}";
}
