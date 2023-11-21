using System;
using System.Collections.Generic;
using System.Reflection;
using Nautilus.Handlers;
using SCHIZO.Registering;
using UnityEngine;

namespace SCHIZO.Items.Components;

partial class MiddleClickAction : IPrefabInit
{
    private static readonly Dictionary<TechType, MethodInfo> _registered = [];

    void IPrefabInit.PrefabInit()
    {
        TechType techType = CraftData.GetTechType(gameObject);
        if (_registered.ContainsKey(techType))
        {
            LOGGER.LogWarning($"Middle click actions already registered for {techType}, skipping");
            return;
        }

        MethodInfo methodInfo = target.GetType().GetMethod(method);
        _registered[techType] = methodInfo;
        if (methodInfo is { })
        {
            ItemActionHandler.RegisterMiddleClickAction(techType, Invoke, displayText);
        }
        else
        {
            LOGGER.LogError($"{name} could not find a method called '{method}' on {target.GetType()}! {nameof(MiddleClickAction)} will not work");
        }
    }

    private void Invoke(InventoryItem item)
    {
        MethodInfo mi = _registered[item.techType];
        ParameterInfo[] args = mi.GetParameters();

        object[] parameters = new object[args.Length];
        int limit = Mathf.Min(args.Length, arguments.Length);
        for (int i = 0; i < limit; i++)
        {
            parameters[i] = Convert.ChangeType(arguments[i], args[i].ParameterType);
        }
        mi.Invoke(target, parameters);
    }
}
