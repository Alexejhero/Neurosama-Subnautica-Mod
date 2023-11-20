using System.Text;

namespace Immersion.Formatting;

public static class Format
{
    public static string FormatPlayer(string format)
    {
        string player = Globals.PlayerName;
        PronounSet pronouns = Globals.PlayerPronouns;
        return new StringBuilder(format)
            .Replace("{player}", player)
            .Replace("{subject}", pronouns.Subject)
            .Replace("{object}", pronouns.Object)
            .Replace("{possessive}", pronouns.Possessive)
            .Replace("{is}", pronouns.IsContraction)
            .Replace("{has}", pronouns.HasContraction)
            .Replace("{reflexive}", pronouns.Reflexive)
            .ToString();
    }
}
