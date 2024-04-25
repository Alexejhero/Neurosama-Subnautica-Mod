using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace SCHIZO.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
[MeansImplicitUse]
public sealed class InitializeModAttribute : Attribute
{
    public static void Run()
    {
        IEnumerable<MethodInfo> methods = PLUGIN_ASSEMBLY.GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            .Where(m => m.GetCustomAttribute<InitializeModAttribute>() != null);

        foreach (MethodInfo method in methods)
        {
            LOGGER.LogInfo($"Calling initialization method {method.DeclaringType}.{method.Name}");
            method.Invoke(null, null);
        }
    }
}
