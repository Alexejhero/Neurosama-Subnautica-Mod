using System.Reflection;
using HarmonyLib;
using NaughtyAttributes;
using NaughtyAttributes.Editor;
using SCHIZO.Attributes.Validation;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts.NaughtyExtensions.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(SwitchDropdownAttribute), true)]
    public sealed class SwitchDropdownAttributeDrawer : DropdownPropertyDrawer
    {
        private static readonly FieldInfo _m_Attribute_field = AccessTools.Field(typeof(PropertyDrawer), "m_Attribute");

        private PropertyAttribute m_Attribute
        {
            set => _m_Attribute_field.SetValue(this, value);
        }

        protected override float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
        {
            SwitchDropdownAttribute actualAttribute = (SwitchDropdownAttribute) attribute;
            m_Attribute = new DropdownAttribute(actualAttribute.GetDropdownListName(property));

            try
            {
                return base.GetPropertyHeight_Internal(property, label);
            }
            finally
            {
                m_Attribute = actualAttribute;
            }
        }

        protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
        {
            SwitchDropdownAttribute actualAttribute = (SwitchDropdownAttribute) attribute;
            m_Attribute = new DropdownAttribute(actualAttribute.GetDropdownListName(property));

            try
            {
                base.OnGUI_Internal(rect, property, label);
            }
            finally
            {
                m_Attribute = actualAttribute;
            }
        }
    }
}
