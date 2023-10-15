using NaughtyAttributes.Editor;
using SCHIZO.Packages.NaughtyAttributes;
using SCHIZO.Utilities;
using UnityEditor;

namespace NaughtyExtensions
{
    public class RequiredSubnauticaReferencePropertyValidator : RequiredPropertyValidator
    {
        public override void ValidateProperty(SerializedProperty property)
        {
            RequiredSubnauticaReferenceAttribute requiredAttribute = PropertyUtility.GetAttribute<RequiredSubnauticaReferenceAttribute>(property);

            if (property.propertyType == SerializedPropertyType.Generic && property.type.Contains(nameof(SubnauticaReference)))
            {
                if (property.FindPropertyRelative(nameof(SubnauticaReference.value)) == null)
                {
                    string errorMessage = property.name + " is required";
                    if (!string.IsNullOrEmpty(requiredAttribute.Message))
                    {
                        errorMessage = requiredAttribute.Message;
                    }

                    NaughtyEditorGUI.HelpBox_Layout(errorMessage, MessageType.Error, context: property.serializedObject.targetObject);
                }
            }
            else
            {
                string warning = requiredAttribute.GetType().Name + " works only on SubnauticaReference types";
                NaughtyEditorGUI.HelpBox_Layout(warning, MessageType.Warning, context: property.serializedObject.targetObject);
            }
        }
    }
}
