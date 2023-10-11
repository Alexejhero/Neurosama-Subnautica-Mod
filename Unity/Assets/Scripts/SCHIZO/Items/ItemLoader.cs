using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SCHIZO.Unity.Items
{
    public abstract class ItemLoader : ScriptableObject
    {
        [NaughtyAttributes.ReadOnly] public CloneItemData itemData;

        public abstract void Load();
    }
}
