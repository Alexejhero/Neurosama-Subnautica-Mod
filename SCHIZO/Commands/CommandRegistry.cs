using System;
using System.Collections.Generic;

namespace SCHIZO.Commands;

internal static class CommandRegistry
{
    public static Dictionary<string, Command> Commands = new(StringComparer.OrdinalIgnoreCase);

    public static void Register(Command command)
    {
        if (Commands.ContainsKey(command.Name))
            LOGGER.LogWarning($"Command {command.Name} already registered, overwriting with new");
        Commands[command.Name] = command;
    }
}
