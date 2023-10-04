using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Nautilus.Handlers;

namespace SCHIZO.API.Attributes;

[AttributeUsage(AttributeTargets.Class), MeansImplicitUse]
public sealed class LoadConsoleCommandsAttribute : Attribute
{
    public static void RegisterAll()
    {
        MAIN_ASSEMBLY.GetTypes()
            .Where(t => t.GetCustomAttribute<LoadConsoleCommandsAttribute>() != null)
            .ForEach(ConsoleCommandsHandler.RegisterConsoleCommands);
    }
}
