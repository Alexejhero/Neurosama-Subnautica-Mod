using System;
using System.Collections.Generic;
using System.Reflection;
using SCHIZO.Commands.Attributes;

namespace SCHIZO.Commands.Base;

public static class CommandRegistry
{
    private const BindingFlags FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    public static Dictionary<string, Command> Commands = new(StringComparer.OrdinalIgnoreCase);
    public static Dictionary<string, List<Command>> Categories = [];

    public static void Register(Command command, string category = "Uncategorized")
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        if (string.IsNullOrEmpty(category))
            throw new ArgumentNullException(nameof(category));

        if (Commands.ContainsKey(command.Name))
            LOGGER.LogWarning($"Command {command.Name} already registered, overwriting with new");
        LOGGER.LogDebug($"Registering {command.GetType().Name} {command.Name}");
        Commands[command.Name] = command;
        Categories.GetOrAddNew(category).Add(command);
    }

    /// <summary>
    /// Register the given <paramref name="command"/> as an in-game console command.<br/>
    /// </summary>
    public static void RegisterConsoleCommand(Command command)
    {
        NautilusCommandWrapper.Register(command);
    }

    public static bool TryGetCommand(string name, out Command command)
        => Commands.TryGetValue(name, out command);

    public static void RegisterAttributeDeclarations(Assembly assembly)
    {
        foreach (Type type in assembly.GetTypes())
        {
            CommandAttribute commandAttr = type.GetCustomAttribute<CommandAttribute>();
            CommandCategoryAttribute categoryAttr = type.GetCustomAttribute<CommandCategoryAttribute>();
            // todo pull the two methods below into this one (yes a megamethod)
            // there are interactions/combos between the two attributes (e.g. [Command]+[CommandCategory] : Command)
            if (commandAttr is { })
            {
                RegisterCommand(type, commandAttr);
            }
            else if (categoryAttr is { })
            {
                RegisterCategory(type, categoryAttr);
            }
            // todo register nested types ([Command] under static [CommandCategory], or [SubCommand] under CompositeCommand)
        }
    }

    private static void RegisterCategory(Type type, CommandCategoryAttribute attr)
    {
        bool derivesFromCommand = typeof(Command).IsAssignableFrom(type);
        if (derivesFromCommand)
        {
            LOGGER.LogDebug($"Registering single {type.Name} to {attr.Category}");
            Register((Command)Activator.CreateInstance(type), attr.Category);
            return;
        }

        foreach (MethodInfo method in type.GetMethods(FLAGS))
        {
            if (method.GetCustomAttribute<CommandAttribute>() is not { } commandAttr)
                continue;

            if (!method.IsStatic)
            {
                LOGGER.LogError($"""
                    {commandAttr.GetType().Name} on {method.Name} in {type.FullName} will be skipped.
                    Under {nameof(CommandCategoryAttribute)}, all methods decorated with {nameof(CommandAttribute)} must be static.
                    """);
                continue;
            }

            Command command = Command.FromStaticMethod(method);
            command.SetInfo(commandAttr);
            Register(command, attr.Category);

            if (commandAttr.RegisterConsoleCommand)
                RegisterConsoleCommand(command);
        }
    }

    private static void RegisterCommand(Type type, CommandAttribute commandAttr)
    {
        bool derivesFromCommand = typeof(Command).IsAssignableFrom(type);

        if (!derivesFromCommand)
        {
            LOGGER.LogError($"""
                {type.FullName} will not be registered as a command.
                Commands decorated with {nameof(CommandAttribute)} must derive from {nameof(Command)}.
                """);
            return;
        }
        Command command = (Command)Activator.CreateInstance(type);
        command.SetInfo(commandAttr);
        if (command is CompositeCommand composite)
        {
            // add subcommands
            foreach (MethodInfo method in type.GetMethods(FLAGS))
            {
                if (method.GetCustomAttribute<SubCommandAttribute>() is not { } subcommandAttr)
                    continue;
                Command subcommand = method.IsStatic
                    ? Command.FromStaticMethod(method)
                    : Command.FromInstanceMethod(method, command);
                subcommand.SetInfo(subcommandAttr, method.Name.ToLowerInvariant());
                composite.AddSubCommand(subcommand);
            }

            Register(command, command.Name);
        }

        if (commandAttr.RegisterConsoleCommand)
            RegisterConsoleCommand(command);
    }
}
