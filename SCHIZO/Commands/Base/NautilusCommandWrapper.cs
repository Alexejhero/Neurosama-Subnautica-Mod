using System.Collections.Generic;
using System.Linq;
using Nautilus.Handlers;
using SCHIZO.Commands.Context;

namespace SCHIZO.Commands.Base;

/// <summary>
/// Wrapper around a <see cref="Command"/> to register it as an in-game console command using Nautilus's <see cref="ConsoleCommandsHandler"/>.
/// </summary>
public sealed class NautilusCommandWrapper
{
    private static readonly Dictionary<string, NautilusCommandWrapper> _registered = [];
    public Command Command { get; }

    private NautilusCommandWrapper(Command command)
    {
        Command = command;
    }

    public static NautilusCommandWrapper Register(Command command)
    {
        if (_registered.ContainsKey(command.Name))
        {
            LOGGER.LogError($"Tried to register {command.Name} twice, skipping duplicate registration");
            return null;
        }
        if (command.Name.Any(c => !IsValidConsoleInput(c)))
        {
            LOGGER.LogWarning($"""
                Registering console wrapper for command '{command.Name}' but its name has characters you can't type into the console. Did you mean to do this?
                Allowed characters are A-Z, a-z, 0-9, space, and -_./:()
                """);
        }
        NautilusCommandWrapper wrapper = new(command);
        wrapper.RegisterWithNautilus();
        return wrapper;
    }

    // needed bc "params" gets ignored on lambdas
    private void RegisterWithNautilus()
    {
        ConsoleCommandsHandler.RegisterConsoleCommand(Command.Name, Execute);
    }

    private object Execute(params string[] args)
    {
        string input = string.Join(" ", args.Prepend(Command.Name));
        ConsoleCommandContext ctx = new()
        {
            Command = Command,
            Input = new Input.StringInput(input) { Command = Command },
            Output = new(),
        };
        ctx.Output.ModifyForConsoleNautilus(ctx);
        ctx.Output.AddCommonResultTransformers(Command);
        Command.Execute(ctx);
        return ctx.Result;
    }

    private static readonly HashSet<char> _specialAllowedCharacters = new(" -_./:()");
    private static bool IsValidConsoleInput(char c)
    {
        return c is >= 'A' and <= 'Z'
                or >= 'a' and <= 'z'
                or >= '0' and <= '9'
            || _specialAllowedCharacters.Contains(c);
    }
}
