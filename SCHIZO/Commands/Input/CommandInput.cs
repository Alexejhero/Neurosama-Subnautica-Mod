using System.Collections.Generic;
using SCHIZO.Commands.Base;

namespace SCHIZO.Commands.Input;

#nullable enable
// it's abstract because different execution sources have different ways of formatting arguments
// (e.g. console is just one big ol' string, and web is JSON where arguments are named)
public abstract class CommandInput
{
    public required Command Command { get; init; }

    /// <summary>Represent the input arguments as a single string a-la console input (e.g. <c>"abc 123 foo bar"</c>).</summary>
    public virtual string AsConsoleString()
        => $"{Command.Name} {string.Join(" ", GetPositionalArguments())}";
    public abstract IEnumerable<object?> GetPositionalArguments();

    public abstract string? GetSubCommandName();
    public abstract CommandInput GetSubCommandInput(Command subCommand);
}
