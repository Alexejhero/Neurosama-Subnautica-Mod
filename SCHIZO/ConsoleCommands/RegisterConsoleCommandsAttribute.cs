using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Nautilus.Commands;
using Nautilus.Handlers;

namespace SCHIZO.ConsoleCommands;

[AttributeUsage(AttributeTargets.Class), MeansImplicitUse]
internal sealed class RegisterConsoleCommandsAttribute : Attribute
{
    public static void RegisterAll()
    {
        PLUGIN_ASSEMBLY.GetTypes()
            .Where(t => t.GetCustomAttribute<RegisterConsoleCommandsAttribute>() != null)
            .ForEach(ConsoleCommandsHandler.RegisterConsoleCommands);
    }
}
