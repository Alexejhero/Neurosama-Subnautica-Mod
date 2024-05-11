using Immersion.Formatting;

namespace Immersion.Trackers;

public sealed class NewScans : Tracker
{
    private void Start()
    {
        PDAScanner.onAdd += OnScanned;
    }

    public void OnScanned(PDAScanner.Entry entry)
    {
        NotifyScanned(entry.techType);
    }

    internal void NotifyScanned(TechType techType)
    {
        string name = Language.main.TryGet(techType, out string nameMaybe) ? nameMaybe : Format.ToTitleCase(techType);
        string message = Format.FormatPlayer($"{{player}} has discovered a new creature: {name}.");

        string encyKey = PDAScanner.GetEntryData(techType).encyclopedia;
        if (!PDAEncyclopedia.GetEntryData(encyKey, out PDAEncyclopedia.EntryData data)) return;

        // creatures only
        if (!data.path.StartsWith("Research/Lifeforms/Fauna/")) return;

        React(Priority.Low, message);
    }

    private void OnDestroy()
    {
        PDAScanner.onAdd -= OnScanned;
    }
}
