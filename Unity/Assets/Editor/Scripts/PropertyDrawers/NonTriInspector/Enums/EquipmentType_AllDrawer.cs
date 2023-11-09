using SCHIZO.Interop.Subnautica.Enums;
using UnityEditor;

namespace Editor.Scripts.PropertyDrawers.NonTriInspector.Enums
{
    [CustomPropertyDrawer(typeof(EquipmentType_All))]
    public sealed class EquipmentType_AllDrawer : GameSpecificEnumDrawer<EquipmentType_All>
    {
    }
}
