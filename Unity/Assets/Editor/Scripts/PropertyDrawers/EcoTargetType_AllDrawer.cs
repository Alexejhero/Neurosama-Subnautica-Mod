using SCHIZO.Interop.Subnautica.Enums;
using UnityEditor;

namespace Editor.Scripts.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(EcoTargetType_All))]
    public sealed class EcoTargetType_AllDrawer : GameSpecificEnumDrawer<EcoTargetType_All>
    {
    }
}
