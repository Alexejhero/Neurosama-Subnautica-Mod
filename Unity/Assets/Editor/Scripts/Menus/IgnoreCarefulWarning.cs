using UnityEditor;

namespace Editor.Scripts.Menus
{
    public static class IgnoreCarefulWarning
    {
        private const string MENU_NAME = "Tools/SCHIZO/Ignore Careful Warning";
        private const string EDITOR_PREFS_KEY = "SCHIZO_IgnoreCarefulWarning";

        public static bool Enabled
        {
            get => EditorPrefs.GetBool(EDITOR_PREFS_KEY, false);
            private set => EditorPrefs.SetBool(EDITOR_PREFS_KEY, value);
        }

        [MenuItem(MENU_NAME)]
        private static void Toggle()
        {
            Enabled = !Enabled;
        }

        [MenuItem(MENU_NAME, true)]
        private static bool ToggleValidate()
        {
            Menu.SetChecked(MENU_NAME, Enabled);
            return true;
        }
    }
}
