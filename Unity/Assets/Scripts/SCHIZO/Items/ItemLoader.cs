using UnityEngine;

namespace SCHIZO.Items
{
    public abstract class ItemLoader : ScriptableObject
    {
        [NaughtyAttributes.ReadOnly] public CloneItemData itemData;

        public abstract void Load();
    }
}
