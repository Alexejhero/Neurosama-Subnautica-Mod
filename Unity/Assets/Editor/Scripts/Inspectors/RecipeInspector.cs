using Editor.Scripts.PropertyDrawers.NonTriInspector.Enums;
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

        protected override void OnEnable()
        {
            base.OnEnable();

            Recipe recipe = (Recipe) target;
            TechType_AllDrawer.TargetGame = recipe.game;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            TechType_AllDrawer.TargetGame = 0;
        }
    }
}
