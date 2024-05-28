using System.Collections.Generic;
using System.Linq;
using Nautilus.Handlers;

namespace SCHIZO.Commands.Base;

/// <summary>
/// Wrapper around a <see cref="Command"/> to register it as an in-game console command using Nautilus's <see cref="ConsoleCommandsHandler"/>.
/// </summary>
public sealed class NautilusCommandWrapper
{
    private static readonly Dictionary<string, NautilusCommandWrapper> _registry = [];
    public Command Command { get; }

    private NautilusCommandWrapper(Command command)
    {
        Command = command;
    }

    public static NautilusCommandWrapper Register(Command command)
    {
        if (_registry.TryGetValue(command.Name, out NautilusCommandWrapper existing))
        {
            LOGGER.LogError($"Tried to register {command.Name} twice, skipping duplicate registration");
            return null;
        }
        NautilusCommandWrapper wrapper = new(command);
        wrapper.RegisterSelf();
        return wrapper;
    }

    // needed bc "params" gets ignored on lambdas
    private void RegisterSelf()
    {
        ConsoleCommandsHandler.RegisterConsoleCommand(Command.Name, NewMethod);
    }

    private object NewMethod(params string[] args)
    {
        Input.ConsoleInput input = new(string.Join(" ", args.Prepend(Command.Name)));
        CommandExecutionContext ctx = new()
        {
            RootCommand = Command,
            Input = input,
            Output = new(),
        };
        ctx.Output.ModifyForConsoleNautilus(ctx);
        Command.Execute(ctx);
        return ctx.Result;
    }
}
