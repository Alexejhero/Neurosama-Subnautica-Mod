using SCHIZO.Items.Data;
using UnityEngine;

namespace SCHIZO.Items
{
    public abstract partial class CloneItemLoader : ScriptableObject
    {
        [NaughtyAttributes.ReadOnly] public CloneItemData itemData;
    }
}
