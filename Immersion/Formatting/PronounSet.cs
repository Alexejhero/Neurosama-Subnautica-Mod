using System.Diagnostics;

namespace Immersion.Formatting;

/// <summary>
/// A set of pronouns.
/// </summary>
/// <param name="Subject">When used as the subject in a sentence. E.g. "<b>She</b> has completed the project." </param>
/// <param name="Object">When used as the object in a sentence. E.g. "The task stumped <b>him</b>."</param>
/// <param name="Possessive">Possessive pronoun. E.g. "<b>Its</b> health was dangerously low."</param>
/// <param name="IsContraction">Contraction of "X is". E.g. "<b>They're</b> the focus."</param>
/// <param name="HasContraction">Contraction of "X has". E.g. "<b>I've</b> done it."</param>
/// <param name="Reflexive">Reflects back to the subject. E.g. "It seems they've placed <b>themself</b> in a precarious position."</param>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly record struct PronounSet(string Subject, string Object, string Possessive, string IsContraction, string HasContraction, string Reflexive)
{
    #region Predefined
    public static readonly PronounSet IMe = new("I", "me", "my", "I'm", "I've", "myself");
    public static readonly PronounSet You = new("you", "you", "your", "you're", "you've", "yourself");
    public static readonly PronounSet HeHim = new("he", "him", "his", "he's", "he's", "himself");
    public static readonly PronounSet SheHer = new("she", "her", "her", "she's", "she's", "herself");
    public static readonly PronounSet TheyThem = new("they", "them", "their", "they're", "they've", "themself");
    public static readonly PronounSet TheyThemPlural = TheyThem with { Reflexive = "themselves" };
    public static readonly PronounSet ItIts = new("it", "it", "its", "it's", "it's", "itself");
    #endregion Predefined

    public static List<PronounSet> DefinedSets = [IMe, You, HeHim, SheHer, TheyThem, TheyThemPlural, ItIts];

    public readonly string[] Parts = [Subject, Object, Possessive, IsContraction, HasContraction, Reflexive];
    public override string ToString() => string.Join("/", Parts);
    private string DebuggerDisplay => $"{{ {Subject}/{Object} }}";

    public static bool TryParse(string formatted, out PronounSet pronouns)
    {
        pronouns = default;
        if (string.IsNullOrEmpty(formatted))
            return false;
        string[] parts = formatted.Split('/');
        foreach (PronounSet existing in DefinedSets)
        {
            // match partial input, e.g. "he" or "she/her"
            string[] existingParts = existing.Parts;
            bool matches = true;
            for (int i = 0; i < parts.Length; i++)
            {
                if (!string.Equals(parts[i], existingParts[i], StringComparison.CurrentCultureIgnoreCase))
                {
                    matches = false;
                    break;
                }
            }
            if (matches)
            {
                pronouns = existing;
                return true;
            }
        }
        // these three are the absolute minimum required
        if (parts is not [string subject, string @object, string possessive, ..])
            return false; // we can't really do much with 2 and below if they're not already defined (caught above) so let's give up
        (string isContraction, string hasContraction, string reflexive) = parts.Length switch
        {
            // we're the happiest with a full set
            6 => (parts[3], parts[4], parts[5]),
            // but we'll do our best to make it work for others too
            // notably, we make a guess that the pronoun set is singular (not plural)

            // reflexive was omitted
            5 => (parts[3], parts[4], @object+"self"),
            // only contractions were omitted
            4 => (subject+"'s", subject+"'s", parts[3]),

            // both contractions and reflexive omitted
            3 => (subject+"'s", subject+"'s", @object+"self"),
            // 7+ - assume malformed
            _ => default,
        };
        if (isContraction == default) return false;
        pronouns = new PronounSet(subject, @object, possessive, isContraction, hasContraction, reflexive);
        return true;
    }
}
