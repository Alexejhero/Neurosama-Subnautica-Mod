using System;
using System.Collections.Generic;
using System.Linq;
using SCHIZO.Commands.Base;
using SCHIZO.Helpers;

namespace SCHIZO.Commands.Input;

public class StringInput : CommandInput
{
    public string InputString { get; }
    public string ArgsString { get; }
    private string[] _splitArgs;

    public StringInput(string input)
    {
        if (string.IsNullOrEmpty(input))
            return;

        InputString = input;
        ArgsString = input.SplitOnce(' ').After;
    }

    public override string AsConsoleString() => InputString;
    public override string GetSubCommandName()
        => ArgsString?.SplitOnce(' ').Before;
    public override IEnumerable<object> GetPositionalArguments()
        => CacheArgs() ?? [];
    public override Dictionary<string, object> GetNamedArguments()
    {
        if (Command is not MethodCommand comm)
            return [];

        MethodCommand.ArgParseResult res = comm.TryParsePositionalArgs(CacheArgs());
        return res.ParsedArgs
            .Zip(comm.Parameters, (a, b) => (a, b))
            .ToDictionary(pair => pair.b.Name, pair => pair.a);
    }

    private string[] CacheArgs()
        => _splitArgs ??= ArgsString?.Split([' '], StringSplitOptions.RemoveEmptyEntries);
    public override CommandInput GetSubCommandInput(Command subCommand)
        => new StringInput(ArgsString) { Command = subCommand };
}
