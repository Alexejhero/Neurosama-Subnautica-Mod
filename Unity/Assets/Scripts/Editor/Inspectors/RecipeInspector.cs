using NaughtyAttributes.Editor;
using PropertyDrawers;
using SCHIZO.Unity.Items;
using UnityEditor;

namespace Inspectors
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
