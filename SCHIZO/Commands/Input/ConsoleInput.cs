using System;
using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Helpers;

namespace SCHIZO.Commands.Input;

public class ConsoleInput : CommandInput
{
    public string InputString { get; }
    public string Arguments { get; }
    private string[] _splitArgs;

    public ConsoleInput(string input)
    {
        if (string.IsNullOrEmpty(input))
            return;

        InputString = input;
        Arguments = input.SplitOnce(' ').After;
    }

    public override string AsConsoleString() => InputString;
    public override string GetSubCommandName()
        => Arguments?.SplitOnce(' ').Before;
    public override IEnumerable<object> GetPositionalArguments()
        => CacheArgs() ?? [];

    private string[] CacheArgs()
        => _splitArgs ??= Arguments?.Split([' '], StringSplitOptions.RemoveEmptyEntries);
    public override CommandInput GetSubCommandInput(Command subCommand)
        => new ConsoleInput(Arguments) { Command = subCommand };
}
