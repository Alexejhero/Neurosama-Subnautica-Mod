using System.Collections.Generic;
using System.Linq;
using SCHIZO.Commands.Input;
using SCHIZO.Commands.Output;

namespace SCHIZO.Commands;

/// <summary>
/// Wrapper around a base-game console command.
/// </summary>
public class ConsoleCommandWrapper : Command
{
    public string Command { get; set; }

    protected override void ExecuteCore(CommandExecutionContext ctx)
    {
        IEnumerable<string> parts = ctx.Arguments
            .Select(arg => arg.ToString())
            .Prepend(Command);
        DevConsole.SendConsoleCommand(string.Join(" ", parts));
        ctx.SetResult(new CommonResults.OK());
    }
}
