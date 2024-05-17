using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SCHIZO.Helpers;

public static class GameObjectEnumerableHelpers
{
    public static IEnumerable<GameObject> AllOfTechType(TechType techType)
        => GameObject.FindObjectsOfType<TechTag>()
            .Where(tag => tag.type == techType)
            .Select(tag => tag.gameObject)
        .Concat(GameObject.FindObjectsOfType<PrefabIdentifier>()
            .Where(id => CraftData.GetTechType(id.gameObject, out _) == techType)
            .Select(id => id.gameObject));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<TComponent> SelectComponent<TComponent>(this IEnumerable<GameObject> gameObjects) where TComponent : Component
        => gameObjects
            .Select(gameObj => gameObj.GetComponent<TComponent>())
            .Where(comp => comp);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<TComponent> SelectComponentInParent<TComponent>(this IEnumerable<GameObject> gameObjects) where TComponent : Component
        => gameObjects
            .Select(gameObj => gameObj.GetComponentInParent<TComponent>())
            .Where(comp => comp);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<GameObject> WithComponent<TComponent>(this IEnumerable<GameObject> gameObjects) where TComponent : Component
        => gameObjects.Where(gameObj => gameObj.GetComponent<TComponent>());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<GameObject> OfTechType(this IEnumerable<GameObject> gameObjects, TechType techType)
        => gameObjects.Where(gameObj => CraftData.GetTechType(gameObj) == techType);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<GameObject> OfTechType(this IEnumerable<GameObject> gameObjects, ICollection<TechType> techTypes)
        => gameObjects.Where(gameObj => techTypes.Contains(CraftData.GetTechType(gameObj)));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<GameObject> OfTechType(this IEnumerable<GameObject> gameObjects, IEnumerable<TechType> techTypes)
        => gameObjects.Where(gameObj => techTypes.Contains(CraftData.GetTechType(gameObj)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IOrderedEnumerable<GameObject> OrderByDistanceTo(this IEnumerable<GameObject> gameObjects, Vector3 target)
        => gameObjects.OrderBy(gameObj => gameObj.transform.position.DistanceSqrXZ(target));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IOrderedEnumerable<T> OrderByDistanceTo<T>(this IEnumerable<T> components, Vector3 target)
        where T : Component
        => components.OrderBy(comp => comp.transform.position.DistanceSqrXZ(target));
}
