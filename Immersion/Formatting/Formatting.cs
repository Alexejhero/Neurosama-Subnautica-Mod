using System.Text.RegularExpressions;
using AvsAnLib;
using UnityEngine.UIElements;

namespace Immersion.Formatting;

public static class Format
{
    private static Regex _formatPattern = new(@"\{(\w+)\}", RegexOptions.Compiled);
    private static Regex _titleCasePattern = new(GetTitleCasePattern(), RegexOptions.Compiled);
    public static string FormatPlayer(string format)
    {
        string player = Globals.PlayerName;
        PronounSet pronouns = Globals.PlayerPronouns;
        return _formatPattern.Replace(format, (m) =>
            m.Groups[1].Value switch
            {
                "player" => player,
                "subject" => pronouns.Subject,
                "object" => pronouns.Object,
                "possessive" => pronouns.Possessive,
                "is" => pronouns.IsContraction,
                "has" => pronouns.HasContraction,
                "reflexive" => pronouns.Reflexive,
                _ => m.Value, // keep other format tags like {0}
            }
        );
    }

    private static string GetTitleCasePattern()
    {
        /* lang=regex */
        string endAcronym = "(?<=[A-Z])([A-Z])(?=[a-z])"; // splits "HELLOWorld" into "HELLO World"
        /* lang=regex */
        string startWord = "(?<=[a-z])([A-Z])"; // splits "helloWorld" into "hello World"
        /* lang=regex */
        string numbers = @"(?<=[^\d])([\d]+)|(?<=[\d])([^\d.])"; // splits "bring25.5bottles" into "bring 25.5 bottles"
        
        return $"({endAcronym}|{startWord}|{numbers})";
    }

    private static Dictionary<string, string> _titleCaseCache = [];
    public static string ToTitleCase(string name)
    {
        return _titleCaseCache.TryGetValue(name, out string titleCase) ? titleCase
            : _titleCaseCache[name] = _titleCasePattern.Replace(name.ToPascalCase(), " $0").Trim();
    }

    public static string ToTitleCase<T>(T value) => ToTitleCase(value.ToString());
    public static string WithArticle(string word) => $"{AvsAn.Query(word).Article} {word}";
}
