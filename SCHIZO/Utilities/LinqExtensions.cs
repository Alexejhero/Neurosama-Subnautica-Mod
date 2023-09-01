using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SCHIZO.Utilities;
public static class LinqExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<TComponent> ToComponent<TComponent>(this IEnumerable<GameObject> gameObjects)
        where TComponent : Component
        => gameObjects
            .Select(gameObj => gameObj.GetComponent<TComponent>())
            .Where(comp => comp);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<Component> ToComponent(this IEnumerable<GameObject> gameObjects, Type componentType)
        => gameObjects
            .Select(gameObj => gameObj.GetComponent(componentType))
            .Where(comp => comp);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<GameObject> ThatHaveComponent<TComponent>(this IEnumerable<GameObject> gameObjects)
        where TComponent : Component
        => gameObjects.Where(gameObj => gameObj.GetComponent<TComponent>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<GameObject> ThatHaveComponent(this IEnumerable<GameObject> gameObjects, Type componentType)
        => gameObjects.Where(gameObj => gameObj.GetComponent(componentType));
}
