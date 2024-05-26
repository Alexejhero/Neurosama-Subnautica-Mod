namespace SCHIZO.Helpers;

internal static class StringHelpers
{
    public static (string, string) SplitOnce(this string src, char delimiter)
    {
        // splitting w/o span makes me want to take up woodworking
        int split = src.IndexOf(delimiter);
        return (src[..split], src[(split + 1)..]);
    }
}
