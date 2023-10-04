using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SCHIZO.API.Extensions;

public static class EnsureComponentsExtensions
{
    private static readonly Dictionary<Type, List<FieldInfo>> _componentFieldsCache = new();
    private static readonly Dictionary<Type, FieldInfo> _singletonsCache = new();

    public static void EnsureComponentFields(this GameObject obj) => obj.GetComponents<Component>().ForEach(EnsureComponentFields);
    public static void EnsureComponentFields(this Component comp) => EnsureComponentFields(comp, new HashSet<Component>());

    private static void EnsureComponentFields(Component comp, HashSet<Component> seen)
    {
        if (!comp || seen.Contains(comp)) return;

        if (!_componentFieldsCache.TryGetValue(comp.GetType(), out List<FieldInfo> componentFields))
        {
            componentFields = comp.GetType()
                .GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field => typeof(Component).IsAssignableFrom(field.FieldType)
                    && field.GetCustomAttribute<AssertNotNullAttribute>() != null)
                .ToList();
            _componentFieldsCache[comp.GetType()] = componentFields;
        }
        seen.Add(comp);

        foreach (FieldInfo componentField in componentFields)
        {
            if (!(Component)componentField.GetValue(comp))
            {
                //LOGGER.LogDebug($"{comp.GetType().Name} on \"{comp.name}\" missing field ({componentField.FieldType} {componentField.Name}), adding");
                Component component = comp.gameObject.EnsureComponentInHierarchy(componentField.FieldType);
                if (!component) continue;
                EnsureComponentFields(component, seen);
                componentField.SetValue(comp, component);
            }
        }
    }

    private static Component EnsureComponentInHierarchy(this GameObject obj, Type componentType)
    {
        if (!_singletonsCache.TryGetValue(componentType, out FieldInfo singleton))
        {
            singleton = componentType
                .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(field => field.Name is "main" or "instance" && componentType.IsAssignableFrom(field.FieldType));
            _singletonsCache[componentType] = singleton;
        }

        // not our responsibility to instantiate singletons, bail if it's null
        if (singleton is not null) return singleton.GetValue(null) as Component;

        Component inHierarchy = obj.GetComponentInChildren(componentType);
        if (inHierarchy) return inHierarchy;
        return obj.EnsureComponent(componentType);
    }
}
