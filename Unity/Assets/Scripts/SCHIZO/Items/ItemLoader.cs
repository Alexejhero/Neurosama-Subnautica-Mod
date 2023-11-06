using SCHIZO.Items.Data;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Items
{
    public abstract partial class ItemLoader : ScriptableObject
    {
        [ReadOnly] public ItemData itemData;
    }
}
