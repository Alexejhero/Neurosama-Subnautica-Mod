using System;
using System.Collections.Generic;
using SCHIZO.Helpers;

namespace SCHIZO.Commands.Input;

public class ConsoleInput : CommandInput
{
    private readonly string _input;
    private readonly string _commandName;
    private readonly string _args;
    private string[] _splitArgs;

    public ConsoleInput(string input)
    {
        _input = input;
        (_commandName, _args) = _input.SplitOnce(' ');
    }

    public override string CommandName => _commandName;
    public override IEnumerable<object> Arguments => CacheArgs();
    public override string AsConsoleString() => _input;
    public override CommandInput GetSubCommandInput() => new ConsoleInput(_args);

    private string[] CacheArgs()
    {
        return _splitArgs ??= _args.Split([' '], StringSplitOptions.RemoveEmptyEntries);
    }
}
