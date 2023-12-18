using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Registering;

partial class ComponentAdder
{
    private record struct Target(MethodInfo method, Mode mode);
    private record struct Entry(Type targetType, GameObject prefab);

    private static readonly Dictionary<Target, List<Entry>> _toInstantiate = new();

    protected override void Register()
    {
        if (!game.HasFlag(GAME)) return;

        if (isSingleton)
        {
            Instantiate(prefab, PLUGIN_OBJECT.transform);
            return;
        }

        Type targetType = ReflectionCache.GetType(typeName);
        Type actualTargetType = _isBaseType ? ReflectionCache.GetType(targetTypeName) : targetType;
        if (scanForExisting)
        {
            foreach (Component o in FindObjectsOfType(actualTargetType).Cast<Component>())
            {
                Instantiate(prefab, o.transform);
            }
        }

        MethodInfo targetMethod = AccessTools.Method(targetType, methodName)
            ?? throw new MissingMethodException(typeName, methodName);
        Target target = CreateTarget(targetMethod, mode);
        Entry entry = new(actualTargetType, prefab);

        if (_toInstantiate.TryGetValue(target, out List<Entry> list))
        {
            list.Add(entry);
        }
        else
        {
            DoPatch(target);
            _toInstantiate.Add(target, [entry]);
        }
    }

    private static Target CreateTarget(MethodInfo method, Mode mode)
    {
        return mode switch
        {
            Mode.CoroutineStep0Prefix => new Target(AccessTools.EnumeratorMoveNext(method), mode),
            _ => new Target(method, mode)
        };
    }

    private static void DoPatch(Target target)
    {
        switch (target.mode)
        {
            case Mode.Prefix:
                PrefixPatch.Inject(target.method);
                break;

            case Mode.Postfix:
                PostfixPatch.Inject(target.method);
                break;

            case Mode.CoroutineStep0Prefix:
                CoroutineStep0PrefixPatch.Inject(target.method);
                break;
        }
    }

    private static void AddComponents(Target target, MonoBehaviour instance)
    {
        foreach ((Type targetType, GameObject gameObject) in _toInstantiate[target])
        {
            if (targetType.IsInstanceOfType(instance))
            {
                Instantiate(gameObject, instance.transform);
            }
        }
    }

    private static class PrefixPatch
    {
        public static void Inject(MethodInfo targetMethod)
        {
            HARMONY.Patch(targetMethod, prefix: new HarmonyMethod(AccessTools.Method(typeof(PrefixPatch), nameof(PatchFunc))));
        }

        private static void PatchFunc(MonoBehaviour __instance, MethodInfo __originalMethod)
        {
            AddComponents(new Target(__originalMethod, Mode.Prefix), __instance);
        }
    }

    private static class PostfixPatch
    {
        public static void Inject(MethodInfo targetMethod)
        {
            HARMONY.Patch(targetMethod, postfix: new HarmonyMethod(AccessTools.Method(typeof(PostfixPatch), nameof(PatchFunc))));
        }

        private static void PatchFunc(MonoBehaviour __instance, MethodInfo __originalMethod)
        {
            AddComponents(new Target(__originalMethod, Mode.Postfix), __instance);
        }
    }

    private static class CoroutineStep0PrefixPatch
    {
        public static void Inject(MethodInfo targetMethod)
        {
            HARMONY.Patch(targetMethod, prefix: new HarmonyMethod(AccessTools.Method(typeof(CoroutineStep0PrefixPatch), nameof(PatchFunc))));
        }

        private static void PatchFunc(object __instance, MethodInfo __originalMethod)
        {
            if (GetState(__instance) == 0)
            {
                AddComponents(new Target(__originalMethod, Mode.CoroutineStep0Prefix), GetThis<MonoBehaviour>(__instance));
            }
        }

        private static int GetState(object iEnumerator)
        {
            return (int) AccessTools.Field(iEnumerator.GetType(), "<>1__state").GetValue(iEnumerator);
        }

        private static T GetThis<T>(object iEnumerator)
        {
            return (T) AccessTools.Field(iEnumerator.GetType(), "<>4__this").GetValue(iEnumerator);
        }
    }
}
