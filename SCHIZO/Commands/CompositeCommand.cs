using System;
using System.Collections.Generic;
using System.Linq;
using SCHIZO.Commands.Input;
using SCHIZO.Commands.Output;

namespace SCHIZO.Commands;

public class CompositeCommand : Command
{
    public Dictionary<string, Command> SubCommands { get; protected set; }

    protected override void ExecuteCore(CommandExecutionContext ctx)
    {
        object subCommandObj = ctx.Arguments.FirstOrDefault();
        if (subCommandObj is not string subCommandName)
        {
            ctx.SetResult(new CommonResults.ShowUsage());
            return;
        }
        if (!SubCommands.TryGetValue(subCommandName, out Command subCommand))
        {
            ctx.SetResult(new CommonResults.SubCommandNotFound(subCommandName));
            return;
        }

        CommandExecutionContext subCtx = ctx.GetSubContext(subCommand);
        subCommand.Execute(subCtx);
    }
}
