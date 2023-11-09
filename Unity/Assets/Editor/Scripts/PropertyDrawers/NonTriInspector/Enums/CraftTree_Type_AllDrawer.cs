using SCHIZO.Interop.Subnautica.Enums;
using UnityEditor;

namespace Editor.Scripts.PropertyDrawers.NonTriInspector.Enums
{
    [CustomPropertyDrawer(typeof(CraftTree_Type_All))]
    public sealed class CraftTree_Type_AllDrawer : GameSpecificEnumDrawer<CraftTree_Type_All>
    {
    }
}
