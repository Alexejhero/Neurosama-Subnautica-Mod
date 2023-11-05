using Editor.Scripts.PropertyDrawers;
using SCHIZO.Items.Data.Crafting;
using TriInspector.Editors;
using UnityEditor;

namespace Editor.Scripts.Inspectors
{
    [CustomEditor(typeof(Recipe))]
    public sealed class RecipeInspector : TriEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Recipe recipe = (Recipe) target;
            TechType_AllDrawer.TargetGame = recipe.game;
        }

        private void OnEnable()
        {
            Recipe recipe = (Recipe) target;
            TechType_AllDrawer.TargetGame = recipe.game;
        }

        private void OnDisable()
        {
            TechType_AllDrawer.TargetGame = 0;
        }
    }
}
