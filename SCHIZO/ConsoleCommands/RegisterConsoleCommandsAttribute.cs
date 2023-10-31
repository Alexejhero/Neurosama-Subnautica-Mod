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
    static RegisterConsoleCommandsAttribute()
    {
        Parameter.TypeConverters.Add(typeof(bool?), str =>
        {
            if (bool.TryParse(str, out bool result)) return result;
            if (str.Equals("null", StringComparison.OrdinalIgnoreCase)) return null;
            throw new FormatException($"Cannot convert \"{str}\" to bool?");
        });
    }

    public static void RegisterAll()
    {
        PLUGIN_ASSEMBLY.GetTypes()
            .Where(t => t.GetCustomAttribute<RegisterConsoleCommandsAttribute>() != null)
            .ForEach(ConsoleCommandsHandler.RegisterConsoleCommands);
    }
}
