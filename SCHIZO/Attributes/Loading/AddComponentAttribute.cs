using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace SCHIZO.Attributes.Loading;

[AttributeUsage(AttributeTargets.Class), MeansImplicitUse]
[HarmonyPatch]
public sealed class AddComponentAttribute(AddComponentAttribute.Target target) : Attribute
{
    public enum Target
    {
        Plugin,
        Player,
    }

    private readonly Target _target = target;

    public static void AddAll(GameObject gameObject, Target target)
    {
        Assembly.GetExecutingAssembly().GetTypes()
            .Select(t => (t, t.GetCustomAttribute<AddComponentAttribute>()))
            .Where(t => t.Item2 != null && t.Item2._target == target)
            .ForEach(t => gameObject.AddComponent(t.Item1));
    }

    [HarmonyPatch]
    public static class Patches
    {
        [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
        [HarmonyPrefix]
        public static void AddToPlayerPatch(Player __instance)
        {
            AddAll(__instance.gameObject, Target.Player);
        }
    }
}
