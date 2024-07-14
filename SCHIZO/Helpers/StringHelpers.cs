namespace SCHIZO.Helpers;

internal static class StringHelpers
{
    /// <summary>
    /// Splits a string on the first occurrence of a character.
    /// </summary>
    /// <param name="delimiter">Character to split on.</param>
    /// <returns>
    /// A tuple containing the two strings before and after the first occurence of the <paramref name="delimiter"/>.<br/>
    /// If the <paramref name="delimiter"/> does not occur in the string, the tuple will contain the original string and an empty string.
    /// </returns>
    public static (string Before, string After) SplitOnce(this string src, char delimiter)
    {
        // splitting w/o span makes me want to take up woodworking
        int split = src.IndexOf(delimiter);

        if (split < 0 || split == src.Length - 1)
            return (src, "");

        return (src[..split], src[(split + 1)..]);
    }
}
