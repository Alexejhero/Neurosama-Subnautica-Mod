using System.Collections.Generic;
using System.Linq;
using SCHIZO.Commands.Base;
using SCHIZO.Helpers;

namespace SCHIZO.Commands.Input;

#nullable enable
public class JsonInput : CommandInput
{
    public required JsonInputModel Model { get; init; }

    public override string GetSubCommandName()
        => Model.Command.Split([' '], 3).ElementAtOrDefault(1);

    public override IEnumerable<object?> GetPositionalArguments()
    {
        if (Model.Args is not { Count: > 0 }) return [];
        if (Command is not MethodCommand comm) return [];

        return comm.Parameters
            .Select(p => Model.Args.GetOrDefault(p.Name));
    }

    public override CommandInput GetSubCommandInput(Command subCommand)
        => new JsonInput()
        {
            Command = subCommand,
            Model = Model with { Command = Model.Command.SplitOnce(' ').After }
        };
}
