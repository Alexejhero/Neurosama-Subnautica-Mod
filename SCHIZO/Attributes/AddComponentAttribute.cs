using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace SCHIZO.Attributes;

[AttributeUsage(AttributeTargets.Class), MeansImplicitUse]
public sealed class AddComponentAttribute : Attribute
{
    public enum Target
    {
        Plugin,
        Player,
    }

    private readonly Target _target;

    public AddComponentAttribute(Target target)
    {
        _target = target;
    }

    public static void AddAll(GameObject gameObject, Target target)
    {
        Assembly.GetExecutingAssembly().GetTypes()
            .Select(t => (t, t.GetCustomAttribute<AddComponentAttribute>()))
            .Where(t => t.Item2 != null && t.Item2._target == target)
            .ForEach(t => gameObject.AddComponent(t.Item1));
    }
}

[HarmonyPatch]
public static class AddComponentAttributePatches
{
    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    [HarmonyPrefix]
    public static void AddToPlayerPatch(Player __instance)
    {
        AddComponentAttribute.AddAll(__instance.gameObject, AddComponentAttribute.Target.Player);
    }
}
