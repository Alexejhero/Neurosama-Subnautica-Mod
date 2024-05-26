using System.Collections.Generic;

namespace SCHIZO.Commands.Input;

// it's abstract because different execution sources have different ways of formatting arguments
// (e.g. console is just one big ol' string, and web is JSON)
public abstract class CommandInput
{
    public abstract string CommandName { get; }
    public abstract IEnumerable<object> Arguments { get; }

    /// <summary>Represent the input arguments as a single string a-la console input (e.g. <c>"abc 123 foo bar"</c>).</summary>
    public abstract string AsConsoleString();

    /// <summary>
    /// Returns a <see cref="CommandInput"/> that takes the first argument as a <see cref="CommandName">command name</see>.
    /// </summary>
    public abstract CommandInput GetSubCommandInput();
}
