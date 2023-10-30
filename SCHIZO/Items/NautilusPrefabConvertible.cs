using System;
using System.Collections;
using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace SCHIZO.Items;

public sealed class NautilusPrefabConvertible
{
    private readonly int _which;
    private readonly Func<IOut<GameObject>, IEnumerator> _1;
    private readonly PrefabTemplate _2;
    private readonly GameObject _3;
    private readonly Func<GameObject> _4;

    private NautilusPrefabConvertible(Func<IOut<GameObject>, IEnumerator> func)
    {
        _which = 1;
        _1 = func;
    }

    private NautilusPrefabConvertible(PrefabTemplate template)
    {
        _which = 2;
        _2 = template;
    }

    private NautilusPrefabConvertible(GameObject gameObject)
    {
        _which = 3;
        _3 = gameObject;
    }

    private NautilusPrefabConvertible(Func<GameObject> func)
    {
        _which = 4;
        _4 = func;
    }

    public void AssignToGameObject(CustomPrefab nautilusPrefab)
    {
        switch (_which)
        {
            case 1:
                nautilusPrefab.SetGameObject(_1);
                break;

            case 2:
                nautilusPrefab.SetGameObject(_2);
                break;

            case 3:
                nautilusPrefab.SetGameObject(_3);
                break;

            case 4:
                nautilusPrefab.SetGameObject(_4);
                break;
        }
    }

    public static implicit operator NautilusPrefabConvertible(Func<IOut<GameObject>, IEnumerator> func) => new(func);
    public static implicit operator NautilusPrefabConvertible(PrefabTemplate template) => new(template);
    public static implicit operator NautilusPrefabConvertible(GameObject gameObject) => new(gameObject);
    public static implicit operator NautilusPrefabConvertible(Func<GameObject> func) => new(func);
}

public static class NautilusPrefabConvertibleExtensions
{
    public static void SetGameObject(this CustomPrefab prefab, NautilusPrefabConvertible convertible)
    {
        convertible.AssignToGameObject(prefab);
    }
}
