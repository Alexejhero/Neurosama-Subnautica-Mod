using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Nautilus.Handlers;

namespace SCHIZO.Attributes.Loading;

[AttributeUsage(AttributeTargets.Class), MeansImplicitUse]
[Obsolete]
public sealed class LoadConsoleCommandsAttribute : Attribute
{
    public static void RegisterAll()
    {
        Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetCustomAttribute<LoadConsoleCommandsAttribute>() != null)
            .ForEach(ConsoleCommandsHandler.RegisterConsoleCommands);
    }
}
