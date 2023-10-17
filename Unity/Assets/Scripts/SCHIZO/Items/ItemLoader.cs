using UnityEngine;

namespace SCHIZO.Items
{
    public abstract partial class ItemLoader : ScriptableObject
    {
        [NaughtyAttributes.ReadOnly] public Data.CloneItemData itemData;
    }
}
