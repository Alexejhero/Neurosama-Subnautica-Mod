using System;
using System.Collections.Generic;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Input;
using SCHIZO.Commands.Output;

namespace SCHIZO.Commands.Base;

public class CompositeCommand : Command
{
    public Dictionary<string, Command> SubCommands { get; protected set; } = [];

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        if (ctx.Input.GetSubCommandName() is not string subCommandName)
            return CommonResults.ShowUsage();
        if (!SubCommands.TryGetValue(subCommandName, out Command subCommand))
            return CommonResults.ShowUsage();

        CommandExecutionContext subCtx = ctx.GetSubContext(subCommand);
        subCommand.Execute(subCtx);
        object subResult = subCtx.Result;
        subCtx.Output.ProcessOutput(ref subResult);
        return subResult;
    }

    public void AddSubCommand(Command subCommand)
    {
        if (SubCommands.ContainsKey(subCommand.Name))
            throw new ArgumentException($"Subcommand '{subCommand.Name}' already registered to '{Name}'");
        SubCommands[subCommand.Name] = subCommand;
    }
}
