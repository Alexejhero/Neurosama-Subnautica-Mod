using UnityEngine;

namespace SCHIZO.Items
{
    public abstract partial class CloneItemLoader : ScriptableObject
    {
        [NaughtyAttributes.ReadOnly] public Data.CloneItemData itemData;
    }
}
