using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace SCHIZO.API.Attributes;

[AttributeUsage(AttributeTargets.Class), MeansImplicitUse]
public sealed class LoadComponentAttribute : Attribute
{
    public static void AddAll(GameObject gameObject)
    {
        MAIN_ASSEMBLY.GetTypes()
            .Where(t => t.GetCustomAttribute<LoadComponentAttribute>() != null)
            .ForEach(t => gameObject.AddComponent(t));
    }
}
