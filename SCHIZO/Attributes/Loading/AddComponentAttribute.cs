using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace SCHIZO.Attributes.Loading;

[AttributeUsage(AttributeTargets.Class), MeansImplicitUse]
[HarmonyPatch]
[Obsolete]
internal sealed class AddComponentAttribute : Attribute
{
    public static void AddAll(GameObject gameObject)
    {
        Assembly.GetExecutingAssembly().GetTypes()
            .Select(t => (t, t.GetCustomAttribute<AddComponentAttribute>()))
            .Where(t => t.Item2 != null)
            .ForEach(t => gameObject.AddComponent(t.Item1));
    }
}
