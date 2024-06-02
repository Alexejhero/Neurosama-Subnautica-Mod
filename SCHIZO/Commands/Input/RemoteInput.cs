using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Control.Models.Game.Messages;
using SCHIZO.Commands.Base;
using SCHIZO.Helpers;

namespace SCHIZO.Commands.Input;

#nullable enable
public class RemoteInput : CommandInput
{
    public required InvokeCommandMessage Model { get; init; }

    public override string GetSubCommandName()
        => Model.Command.Split([' '], 3).ElementAtOrDefault(1);

    public override IEnumerable<object?> GetPositionalArguments()
    {
        if (Model.Args is not { Count: > 0 }) yield break;
        if (Command is not MethodCommand comm) yield break;

        foreach (ParameterInfo p in comm.Parameters)
        {
            if (Model.Args.TryGetValue(p.Name, out object? value))
                yield return value;
        }
    }

    public override CommandInput GetSubCommandInput(Command subCommand)
        => new RemoteInput()
        {
            Command = subCommand,
            Model = Model with { Command = Model.Command.SplitOnce(' ').After }
        };
}
