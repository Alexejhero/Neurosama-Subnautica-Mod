using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using SCHIZO.API.Creatures;

namespace SCHIZO.API.Attributes;

[AttributeUsage(AttributeTargets.Class), MeansImplicitUse]
public sealed class LoadCreatureAttribute : Attribute
{
    public static void RegisterAll()
    {
        MAIN_ASSEMBLY.GetTypes()
            .Where(t => t.GetCustomAttribute<LoadCreatureAttribute>() != null)
            .Select(Activator.CreateInstance)
            .Cast<ICustomCreatureLoader>()
            .ForEach(l => l.Register());
    }
}
