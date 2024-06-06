using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.SwarmControl.Redeems;

namespace SCHIZO.Commands.ConsoleCommands;

/// <summary>
/// Wrapper around a base-game console command.
/// </summary>
public abstract class ConsoleWrapperCommand(string commandName) : Command, IParameters
{
    public string Command { get; } = commandName;
    public abstract IReadOnlyList<Parameter> Parameters { get; }

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        DevConsole.SendConsoleCommand($"{Command} {string.Join(" ", ctx.Input.GetPositionalArguments())}");
        return null;
    }
}
