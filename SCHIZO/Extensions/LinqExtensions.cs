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


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<GameObject> OfTechType(this IEnumerable<GameObject> gameObjects, TechType techType)
        => gameObjects.Where(gameObj => CraftData.GetTechType(gameObj) == techType);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<GameObject> OfTechType(this IEnumerable<GameObject> gameObjects, ICollection<TechType> techTypes)
        => gameObjects.Where(gameObj => techTypes.Contains(CraftData.GetTechType(gameObj)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IOrderedEnumerable<GameObject> OrderByDistanceTo(this IEnumerable<GameObject> gameObjects, GameObject target)
        => gameObjects.OrderBy(gameObj => gameObj.transform.position.DistanceSqrXZ(target.transform.position));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IOrderedEnumerable<GameObject> OrderByDistanceTo(this IEnumerable<GameObject> gameObjects, Vector3 target)
        => gameObjects.OrderBy(gameObj => gameObj.transform.position.DistanceSqrXZ(target));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IOrderedEnumerable<GameObject> OrderByDistanceToDescending(this IEnumerable<GameObject> gameObjects, GameObject target)
        => gameObjects.OrderByDescending(gameObj => gameObj.transform.position.DistanceSqrXZ(target.transform.position));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IOrderedEnumerable<GameObject> OrderByDistanceToDescending(this IEnumerable<GameObject> gameObjects, Vector3 target)
        => gameObjects.OrderByDescending(gameObj => gameObj.transform.position.DistanceSqrXZ(target));
}
