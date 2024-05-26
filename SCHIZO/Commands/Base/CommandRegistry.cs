using System;
using System.Collections.Generic;

namespace SCHIZO.Commands.Base;

internal static class CommandRegistry
{
    public static Dictionary<string, Command> Commands = new(StringComparer.OrdinalIgnoreCase);
    public static Dictionary<string, List<Command>> Categories = [];

    public static void Register(Command command, string category = "Uncategorized")
    {
        if (Commands.ContainsKey(command.Name))
            LOGGER.LogWarning($"Command {command.Name} already registered, overwriting with new");
        Commands[command.Name] = command;
        Categories.GetOrAddNew(category).Add(command);
    }
}
