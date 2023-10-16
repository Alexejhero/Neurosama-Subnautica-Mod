using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace SCHIZO.Registering;

partial class ComponentAdder
{
    private static readonly Dictionary<MethodInfo, List<GameObject>> _toInstantiate = new();

    public void Patch(GameObject singletonHolder)
    {
        if (isSingleton)
        {
            Instantiate(prefab, singletonHolder.transform);
            return;
        }

        MethodInfo targetMethod = AccessTools.Method($"{typeName}:{methodName}");
        if (targetMethod == null) throw new MissingMethodException(typeName, methodName);

        if (_toInstantiate.TryGetValue(targetMethod, out List<GameObject> list))
        {
            list.Add(prefab);
        }
        else
        {
            HARMONY.Patch(targetMethod, postfix: new HarmonyMethod(AccessTools.Method(typeof(ComponentAdder), nameof(AddComponentPatch))));
            _toInstantiate.Add(targetMethod, new List<GameObject> {prefab});
        }
    }

    private static void AddComponentPatch(MonoBehaviour __instance, MethodInfo __originalMethod)
    {
        _toInstantiate[__originalMethod].ForEach(p => Instantiate(p, __instance.transform));
    }
}
