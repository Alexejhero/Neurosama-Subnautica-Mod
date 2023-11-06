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
        private bool _opened;

        public override void OnGUI(Rect position, TriProperty property, TriElement next)
        {
            Rect propRect = new(position.x, position.y, position.width - 55, position.height);
            Rect buttonRect = new(position.x + position.width - 50, position.y, 50, position.height);

            using (new EditorGUI.DisabledScope(!_opened))
            {
                next.OnGUI(propRect);
            }

            if (!_opened)
            {
                if (GUI.Button(buttonRect, "Edit"))
                {
                    if (EditorUtility.DisplayDialog("Careful!", "This field is not supposed to be changed after it has been set. Are you sure you want to edit it?", "Yes", "No"))
                    {
                        _opened = true;
                    }
                }
            }
            else
            {
                if (GUI.Button(buttonRect, "Done")) _opened = false;
            }
        }
    }
}
