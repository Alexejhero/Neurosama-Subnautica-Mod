using SCHIZO.Commands.Context;

namespace SCHIZO.Commands.Base;

/// <summary>
/// Wrapper around a base-game console command.
/// </summary>
public class ConsoleWrapperCommand : Command
{
    public string Command { get; set; }

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        DevConsole.SendConsoleCommand(ctx.Input.AsConsoleString());
        return null;
    }
}