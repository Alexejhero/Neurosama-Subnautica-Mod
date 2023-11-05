using SCHIZO.Items.Data;
using TriInspector;
using UnityEngine;

namespace SCHIZO.Items
{
    public abstract partial class CloneItemLoader : ScriptableObject
    {
        [ReadOnly] public CloneItemData itemData;
    }
}
