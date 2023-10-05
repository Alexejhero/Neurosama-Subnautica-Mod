using System.Reflection;
using JetBrains.Annotations;
using Nautilus.Handlers;

namespace SCHIZO.Attributes;

[AttributeUsage(AttributeTargets.Class), MeansImplicitUse]
public sealed class LoadConsoleCommandsAttribute : Attribute
{
    public static void RegisterAll()
    {
        Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetCustomAttribute<LoadConsoleCommandsAttribute>() != null)
            .ForEach(ConsoleCommandsHandler.RegisterConsoleCommands);
    }
}
