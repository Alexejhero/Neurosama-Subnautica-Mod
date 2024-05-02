using System.Text.RegularExpressions;
using AvsAnLib;
using UnityEngine.UIElements;

namespace Immersion.Formatting;

public static class Format
{
    private static readonly Regex _formatRegex = new(@"\{(\w+)\}", RegexOptions.Compiled);
    private static readonly Regex _titleCaseRegex = new(GetTitleCasePattern(), RegexOptions.Compiled);
    public static string FormatPlayer(string format)
    {
        string player = Globals.PlayerName;
        PronounSet pronouns = Globals.PlayerPronouns;
        return _formatRegex.Replace(format, (m) =>
            {
                string template = m.Groups[1].Value;
                return template.ToLowerInvariant() switch
                {
                    "player" => player,
                    "subject" => CapitalizeTemplate(template, pronouns.Subject),
                    "object" => CapitalizeTemplate(template, pronouns.Object),
                    "possessive" => CapitalizeTemplate(template, pronouns.Possessive),
                    "is" => CapitalizeTemplate(template, pronouns.IsContraction),
                    "has" => CapitalizeTemplate(template, pronouns.HasContraction),
                    "reflexive" => CapitalizeTemplate(template, pronouns.Reflexive),
                    _ => template, // keep other format tags like {0}
                };
            }
        );
    }

    private static string CapitalizeTemplate(string template, string substitution)
    {
        return char.IsUpper(template[0])
            ? CapitalizeFirst(substitution)
            : substitution;
    }

    public static string CapitalizeFirst(string input)
        => char.ToUpperInvariant(input[0]) + input[1..];

    private static string GetTitleCasePattern()
    {
        /* lang=regex */
        const string endAcronym = "(?<=[A-Z])([A-Z])(?=[a-z])"; // splits "HELLOWorld" into "HELLO World"
        /* lang=regex */
        const string startWord = "(?<=[a-z])([A-Z])"; // splits "helloWorld" into "hello World"
        /* lang=regex */
        const string numbers = @"(?<=[^\d])([\d]+)|(?<=[\d])([^\d.])"; // splits "bring25.5bottles" into "bring 25.5 bottles"

        return $"({endAcronym}|{startWord}|{numbers})";
    }

    private static readonly Dictionary<string, string> _titleCaseCache = [];
    public static string ToTitleCase(string name)
    {
        return _titleCaseCache.TryGetValue(name, out string titleCase) ? titleCase
            : _titleCaseCache[name] = _titleCaseRegex.Replace(name.ToPascalCase(), " $0").Trim();
    }

    public static string ToTitleCase<T>(T value) => ToTitleCase(value.ToString());
    public static string WithArticle(string word) => $"{AvsAn.Query(word).Article} {word}";
}
