using Editor.Scripts.PropertyDrawers.Attributes;
using SCHIZO.Attributes;
using TriInspector;
using UnityEditor;
using UnityEngine;

[assembly: RegisterTriAttributeDrawer(typeof(CarefulAttributeDrawer), TriDrawerOrder.Decorator, ApplyOnArrayElement = true)]

namespace Editor.Scripts.PropertyDrawers.Attributes
{
    internal sealed class CarefulAttributeDrawer : TriAttributeDrawer<CarefulAttribute>
    {
        private TriProperty _opened;

        public override void OnGUI(Rect position, TriProperty property, TriElement next)
        {
            Rect propRect = new(position.x, position.y, position.width - 55, position.height);
            Rect buttonRect = new(position.x + position.width - 50, position.y, 50, position.height);

            using (new EditorGUI.DisabledScope(_opened != property))
            {
                next.OnGUI(propRect);
            }

            using (new EditorGUI.DisabledScope(_opened == property))
            {
                if (GUI.Button(buttonRect, "Edit"))
                {
                    if (Event.current.shift || EditorUtility.DisplayDialog("Careful!", "This field is not supposed to be changed after it has been set. Are you sure you want to edit it?\n(Hold SHIFT to bypass this message in the future.)", "Yes", "No"))
                    {
                        _opened = property;
                    }
                }
            }
        }
    }
}
