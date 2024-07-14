using System.Collections.Generic;
using System.Linq;
using SCHIZO.Commands.Base;
using SCHIZO.Helpers;
using SCHIZO.SwarmControl.Models.Game.Messages;

namespace SCHIZO.Commands.Input;

#nullable enable
public class RemoteInput : CommandInput
{
    public required RedeemMessage Model { get; init; }

    public override string GetSubCommandName()
        => Model.Command.Split([' '], 3).ElementAtOrDefault(1) ?? "";

    public override IEnumerable<object?> GetPositionalArguments()
    {
        if (Model.Args is not { Count: > 0 }) yield break;
        if (Command is not IParameters withParams) yield break;

        foreach (Parameter p in withParams.Parameters)
        {
            if (Model.Args.TryGetValue(p.Name, out object? value))
                yield return value;
            else
                yield return p.DefaultValue;
        }
    }

    public override NamedArgs GetNamedArguments() => new(Model.Args ?? []);

    public override CommandInput GetSubCommandInput(Command subCommand)
        => new RemoteInput
        {
            Command = subCommand,
            Model = Model with { Command = Model.Command.SplitOnce(' ').After }
        };
}
