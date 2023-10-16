using System.Linq;
using NaughtyAttributes.Editor;
using SCHIZO.Registering;
using UnityEditor;
using UnityEngine;

namespace Inspectors
{
    [CustomEditor(typeof(ModRegistryItem), true)]
    public class ModRegistryItemInspector : NaughtyInspector
    {
        private GUIStyle _toggle;

        private void Awake()
        {
            _toggle = new GUIStyle(EditorStyles.toggle);
            _toggle.normal.textColor = Color.red;
            _toggle.hover.textColor = Color.red;
            _toggle.focused.textColor = Color.red;
            _toggle.active.textColor = Color.red;
            _toggle.onNormal.textColor = Color.green;
            _toggle.onHover.textColor = Color.green;
            _toggle.onFocused.textColor = Color.green;
            _toggle.onActive.textColor = Color.green;
        }

        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();

            ModRegistryItem item = target as ModRegistryItem;
            if (!item || item is ModRegistry) return;

            bool isInRegistry = ModRegistry.Instance.registryItems.Contains(item);
            bool isSomehowInRegistry = ModRegistry.Instance.Flatten().Contains(item);

            Rect position = new Rect(46, 24, 105, 15);

            if (!isInRegistry && isSomehowInRegistry)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    GUI.Toggle(position, true, new GUIContent("Add to Registry"), _toggle);
                }

                return;
            }

            EditorGUI.BeginChangeCheck();
            isInRegistry = GUI.Toggle(position, isInRegistry, new GUIContent("Add to Registry"), _toggle);
            if (EditorGUI.EndChangeCheck())
            {
                if (isInRegistry)
                {
                    ModRegistry.Instance.registryItems.Add(item);
                    ModRegistry.Instance.Sort();
                }
                else
                {
                    ModRegistry.Instance.registryItems.RemoveAll(i => i == item);
                }

                EditorUtility.SetDirty(ModRegistry.Instance);
            }
        }

        /*private void DrawDefaultHeader()
        {
            GUILayout.BeginHorizontal(Editor.BaseStyles.inspectorBig);
            GUILayout.Space(38f);
            GUILayout.BeginVertical();
            GUILayout.Space(21f);
            GUILayout.BeginHorizontal();

            if ((bool) (Object) this)
                this.OnHeaderControlsGUI();
            else
                EditorGUILayout.GetControlRect();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            Rect lastRect = GUILayoutUtility.GetLastRect();
            Rect r = new Rect(lastRect.x, lastRect.y, lastRect.width, lastRect.height);
            Rect rect1 = new Rect(r.x + 6f, r.y + 6f, 32f, 32f);
            if ((bool) (Object) this)
                this.OnHeaderIconGUI(rect1);
            else
                GUI.Label(rect1, AssetPreview.GetMiniTypeThumbnail(typeof(Object)), Editor.BaseStyles.centerStyle);
            if ((bool) (Object) this)
                this.DrawPostIconContent(rect1);
            float lineHeight = EditorGUI.lineHeight;
            Rect rect2;
            if ((bool) (Object) this)
            {
                Rect rect3 = this.DrawHeaderHelpAndSettingsGUI(r);
                float x = r.x + 44f;
                rect2 = new Rect(x, r.y + 6f, (float) (rect3.x - (double) x - 4.0), lineHeight);
            }
            else
                rect2 = new Rect(r.x + 44f, r.y + 6f, r.width - 44f, lineHeight);

            if ((bool) (Object) this)
                this.OnHeaderTitleGUI(rect2, this.targetTitle);
            else
                GUI.Label(rect2, this.targetTitle, EditorStyles.largeLabel);
            bool enabled = GUI.enabled;
            GUI.enabled = true;
            Event current = Event.current;
            bool flag = this != null && current.type == EventType.MouseDown && current.button == 1 && r.Contains(current.mousePosition);
            GUI.enabled = enabled;
            if (flag)
            {
                EditorUtility.DisplayObjectContextMenu(new Rect(current.mousePosition.x, current.mousePosition.y, 0.0f, 0.0f), targets, 0);
                current.Use();
            }

        }*/
    }
}
