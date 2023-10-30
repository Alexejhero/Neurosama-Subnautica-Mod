using Editor.Scripts.PropertyDrawers;
using NaughtyAttributes.Editor;
using SCHIZO.Items.Data.Crafting;
using UnityEditor;

namespace Editor.Scripts.Inspectors
{
    [CustomEditor(typeof(Recipe))]
    public sealed class RecipeInspector : NaughtyInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Recipe recipe = (Recipe) target;
            TechType_AllDrawer.TargetGame = recipe.game;
        }

        private new void OnEnable()
        {
            base.OnEnable();
            Recipe recipe = (Recipe) target;
            TechType_AllDrawer.TargetGame = recipe.game;
        }

        private new void OnDisable()
        {
            base.OnDisable();
            TechType_AllDrawer.TargetGame = 0;
        }
    }
}
