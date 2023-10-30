using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SCHIZO.Interop.Subnautica.Enums;
using SCHIZO.Registering;
using UnityEditor;

namespace Editor.Scripts.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(EcoTargetType_All))]
    public sealed class EcoTargetType_AllDrawer : GameSpecificEnumDrawer<EcoTargetType_All>
    {
    }
}
