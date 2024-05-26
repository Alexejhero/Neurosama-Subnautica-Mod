namespace SCHIZO.Commands.Base;

/// <summary>
/// Wrapper around a base-game console command.
/// </summary>
public class ConsoleCommandWrapper : Command
{
    public string Command { get; set; }

    protected override object ExecuteCore(CommandExecutionContext ctx)
    {
        DevConsole.SendConsoleCommand($"{Command} {ctx.Input.AsConsoleString()}");
        return null;
    }
}
