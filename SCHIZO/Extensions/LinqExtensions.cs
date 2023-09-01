using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SCHIZO.Extensions;
public static class LinqExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<TComponent> SelectComponent<TComponent>(this IEnumerable<GameObject> gameObjects)
        where TComponent : Component
        => gameObjects
            .Select(gameObj => gameObj.GetComponent<TComponent>())
            .Where(comp => comp);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<Component> SelectComponent(this IEnumerable<GameObject> gameObjects, Type componentType)
        => gameObjects
            .Select(gameObj => gameObj.GetComponent(componentType))
            .Where(comp => comp);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<GameObject> WithComponent<TComponent>(this IEnumerable<GameObject> gameObjects)
        where TComponent : Component
        => gameObjects.Where(gameObj => gameObj.GetComponent<TComponent>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<GameObject> WithComponent(this IEnumerable<GameObject> gameObjects, Type componentType)
        => gameObjects.Where(gameObj => gameObj.GetComponent(componentType));
}
