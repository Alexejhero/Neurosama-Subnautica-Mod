namespace SCHIZO.Commands.Output;

public static class CommonResults
{
    // sizeof says 1 byte for all of these (type marker i'm guessing)
    public readonly record struct OK;
    public readonly record struct ShowUsage;
    // aaaa i'm having withdrawals i miss rust enums
    public readonly record struct SubCommandNotFound(string name);
}
