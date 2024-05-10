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

    // default timespan format produces garbage like 00:20:00.0580000
    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to a friendly string, e.g. <c>"1 hour, 20 minutes"</c>.
    /// </summary>
    /// <param name="maxComponents">
    /// Maximum number of components (days, hours, minutes, seconds, milliseconds - <b>in this order</b>) to show.<br/>
    /// Zero-valued components will be skipped, i.e. <c>01:00:05</c> will be formatted as <c>"1 hour, 5 seconds"</c>.
    /// </param>
    public static string ToFriendlyString(this TimeSpan ts, int maxComponents = 2)
    {
        // adapted from https://stackoverflow.com/a/7204071
        (int value, string name)[] parts = [
            //(ts.Days / 7, "week"),
            (ts.Days, "day"),
            (ts.Hours, "hour"),
            (ts.Minutes, "minute"),
            (ts.Seconds, "second"),
            //(ts.Milliseconds, "millisecond"),
        ];
        (int value, string name)[] usedParts = parts
            .Where(pair => pair.value > 0)
            .Take(maxComponents)
            .ToArray();
        if (usedParts.Length == 0)
            return "0 seconds";
        return string.Join(", ", usedParts.Select(pair => $"{pair.value} {pair.name}{(pair.value > 1 ? "s" : "")}"));
    }
}
