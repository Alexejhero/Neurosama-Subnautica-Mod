using Immersion.Formatting;

namespace Immersion.Trackers;

public sealed partial class NewScans : Tracker
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

        // creatures only (for now?)
        if (PDAEncyclopedia.ParsePath(data.path) is not ["Research", "Lifeforms", "Fauna", ..]) return;

        // send the first paragraph of the databank entry?
        /*if (Language.main.TryGet($"EncyDesc_{encyKey}", out string description))
        {
            // yep these can have both LF and CRLF
            int firstParagraphEndsAt = description.IndexOfAny(['\r', '\n']);

            string firstParagraph = description;
            if (firstParagraphEndsAt >= 0)
                firstParagraph = description[..firstParagraphEndsAt];

            if (firstParagraph.Length < 150) // :neuroNOspam:
                message += "\n" + firstParagraph.Trim();
        }*/

        React(Priority.Low, message);
    }

    private void OnDestroy()
    {
        PDAScanner.onAdd -= OnScanned;
    }
}
