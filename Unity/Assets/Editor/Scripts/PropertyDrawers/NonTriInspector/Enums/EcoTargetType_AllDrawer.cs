using SCHIZO.Interop.Subnautica.Enums;
using UnityEditor;

namespace Editor.Scripts.PropertyDrawers.NonTriInspector.Enums
{
    [CustomPropertyDrawer(typeof(EcoTargetType_All))]
    public sealed class EcoTargetType_AllDrawer : GameSpecificEnumDrawer<EcoTargetType_All>
    {
    }
}
