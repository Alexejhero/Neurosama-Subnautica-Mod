using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using SCHIZO.Creatures;

namespace SCHIZO.Attributes;

[AttributeUsage(AttributeTargets.Class), MeansImplicitUse]
public sealed class LoadCreatureAttribute : Attribute
{
    public static void RegisterAll()
    {
        Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetCustomAttribute<LoadCreatureAttribute>() != null)
            .Select(Activator.CreateInstance)
            .Cast<ICustomCreatureLoader>()
            .ForEach(l => l.Register());
    }
}
