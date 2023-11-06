using SCHIZO.Items.Data;
using UnityEngine;
using ReadOnlyAttr = TriInspector.ReadOnlyAttribute;

namespace SCHIZO.Items
{
    public abstract partial class ItemLoader : ScriptableObject
    {
        [ReadOnlyAttr] public ItemData itemData;
    }
}
