using SCHIZO.Items.Data;
using UnityEngine;
using ReadOnlyAttr = TriInspector.ReadOnlyAttribute;

namespace SCHIZO.Items
{
    public abstract partial class CloneItemLoader : ScriptableObject
    {
        [ReadOnlyAttr] public CloneItemData itemData;
    }
}
