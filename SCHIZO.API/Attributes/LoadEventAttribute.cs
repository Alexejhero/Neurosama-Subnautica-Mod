using System;
using System.Linq;
using System.Reflection;
using SCHIZO.API.Events;

namespace SCHIZO.API.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class LoadEventAttribute : Attribute
{
    public static void AddAll(CustomEventManager eventManager)
    {
        MAIN_ASSEMBLY.GetTypes()
            .Where(t => t.GetCustomAttribute<LoadEventAttribute>() != null)
            .ForEach(eventManager.AddEvent);
    }
}
