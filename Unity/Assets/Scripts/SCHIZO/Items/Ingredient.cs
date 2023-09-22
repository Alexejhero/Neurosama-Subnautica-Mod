using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Items
{
    [Serializable]
    public sealed class Ingredient
    {
        public TechType techType;
        public int amount;
    }
}
