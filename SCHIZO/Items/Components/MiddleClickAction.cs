using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nautilus.Handlers;
using SCHIZO.Registering;
using UnityEngine;

namespace SCHIZO.Items.Components;

partial class MiddleClickAction : IPrefabInit
{
    private static readonly Dictionary<TechType, MethodInfo> _registered = [];

    void IPrefabInit.PrefabInit(GameObject prefab)
    {
        TechType techType = CraftData.GetTechType(prefab);
        if (_registered.ContainsKey(techType))
        {
            LOGGER.LogWarning($"{nameof(MiddleClickAction)} already registered for {techType}, skipping");
            return;
        }
        string error = Validate(techType);
        if (error is not null)
        {
            LogRegisterError(error);
            return;
        }

        ItemActionHandler.RegisterMiddleClickAction(techType, Invoke, displayText);
    }

    private string Validate(TechType techType)
    {
        if (!target) return $"{nameof(target)} is required and was not provided";

        Type type = target.GetType();
        MethodInfo methodInfo = type.GetMethod(method);
        _registered[techType] = methodInfo;
        if (methodInfo is null)
            return $"could not find a method called '{method}' on {type}";

        ParameterInfo[] args = methodInfo.GetParameters();
        int requiredCount = args.Count(pi => !pi.IsOptional);
        if (requiredCount > arguments.Length)
            return $"method has {requiredCount} required arguments, only {arguments} were provided";
        if (args.Length < arguments.Length)
            return $"{arguments} arguments were provided but the method only has {args.Length}";
        return null;
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

    private void LogRegisterError(string message)
    {
        LOGGER.LogError($"{name} could not register {nameof(MiddleClickAction)} - {message}");
    }
}
