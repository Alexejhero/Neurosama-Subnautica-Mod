using System;
using System.Collections.Generic;
using System.Reflection;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.ConsoleCommands;
using SCHIZO.Helpers;
using SCHIZO.SwarmControl.Redeems;

namespace SCHIZO.Commands.Base;

public static class CommandRegistry
{
    private const BindingFlags FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
    public static Dictionary<string, Command> Commands = new(StringComparer.OrdinalIgnoreCase);
    public static Dictionary<string, List<Command>> Categories = [];
    private static bool _finishedAutoRegistration;

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
        if (_finishedAutoRegistration)
            command.CallPostRegister();
    }

    /// <summary>
    /// Register the given <paramref name="command"/> as an in-game console command.<br/>
    /// </summary>
    public static void RegisterConsoleCommand(Command command)
    {
        NautilusCommandWrapper.Register(command);
    }

    public static bool TryGetInnermostCommand(string name, out Command command)
    {
        (string before, string after) = name.SplitOnce(' ');
        if (!Commands.TryGetValue(before, out command))
            return false;
        while (command is CompositeCommand comp)
        {
            if (!comp.SubCommands.TryGetValue(after, out command))
                return false;
        }
        return true;
    }

    public static void RegisterAttributeDeclarations(Assembly assembly)
    {
        foreach (Type type in assembly.GetTypes())
        {
            CommandAttribute commandAttr = type.GetCustomAttribute<CommandAttribute>();
            CommandCategoryAttribute categoryAttr = type.GetCustomAttribute<CommandCategoryAttribute>();
            // there are interactions/combos between the two attributes
            // (e.g. [Command]+[CommandCategory] : Command) is a single command in a specific category
            if (commandAttr is { })
            {
                bool derivesFromCommand = typeof(Command).IsAssignableFrom(type);
                bool isStaticClass = type.IsAbstract && type.IsSealed;

                if (!derivesFromCommand || isStaticClass)
                {
                    LOGGER.LogWarning($"""
                        {type.FullName} will not be registered as a command.
                        Classes decorated with {nameof(CommandAttribute)} must derive from {nameof(Command)} and be non-static.
                        """);
                    continue;
                }
                Command command = (Command) Activator.CreateInstance(type);
                command.SetInfo(commandAttr);
                if (command is CompositeCommand composite)
                {
                    // composite commands act as a category
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
                else
                {
                    Register(command, categoryAttr?.Category ?? "Uncategorized");
                }

                if (commandAttr is RedeemAttribute redeem)
                {
                    RedeemRegistry.Register(redeem, command);
                }

                if (commandAttr.RegisterConsoleCommand)
                {
                    if (command is ConsoleWrapperCommand concommand && concommand.Command == command.Name)
                    {
                        LOGGER.LogWarning($"""
                            {command.Name} is a {nameof(ConsoleWrapperCommand)} wrapper around a console command of the same name.
                            It will thus not be registered as a console command to avoid collisions.
                            Please remove {nameof(CommandAttribute.RegisterConsoleCommand)} from the attribute or rename the command to no longer collide.
                            """);
                    }
                    else
                    {
                        RegisterConsoleCommand(command);
                    }
                }
            }
            else if (categoryAttr is { })
            {
                RegisterCategory(type, categoryAttr);
            }
            // todo register nested types ([Command] under static [CommandCategory], or [SubCommand] under CompositeCommand)
        }

        Commands.Values.ForEach(command => command.CallPostRegister());
        _finishedAutoRegistration = true;
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
}
