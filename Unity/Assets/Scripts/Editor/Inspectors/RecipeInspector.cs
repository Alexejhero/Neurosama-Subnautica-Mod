using SCHIZO.Unity;
using SCHIZO.Unity.Items;
using UnityEditor;
using UnityEngine.UIElements;

namespace Inspectors
{
    [CustomEditor(typeof(Recipe))]
    public sealed class RecipeInspector : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            Recipe recipe = (Recipe) target;

            VisualElement uxmlContent = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/Inspectors/RecipeInspector.uxml").CloneTree();

            SetupGame(recipe, uxmlContent);

            return uxmlContent;
        }

        private static void SetupGame(Recipe recipe, VisualElement content)
        {
            bool isSN = recipe.game.HasFlag(Game.Subnautica);
            bool isBZ = recipe.game.HasFlag(Game.BelowZero);

            if (!isSN && !isBZ)
            {
                recipe.game = Game.Subnautica | Game.BelowZero;
                isSN = true;
                isBZ = true;
            }

            Toggle subnautica = content.Q<Toggle>("gameSubnautica");
            Toggle belowzero = content.Q<Toggle>("gameBelowZero");

            subnautica.value = isSN;
            subnautica.SetEnabled(isBZ);
            subnautica.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                {
                    recipe.game |= Game.Subnautica;
                    belowzero.SetEnabled(true);
                }
                else
                {
                    recipe.game &= ~Game.Subnautica;
                    belowzero.SetEnabled(false);
                }
            });

            belowzero.value = recipe.game.HasFlag(Game.BelowZero);
            belowzero.SetEnabled(isSN);
            belowzero.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                {
                    recipe.game |= Game.BelowZero;
                    subnautica.SetEnabled(true);
                }
                else
                {
                    recipe.game &= ~Game.BelowZero;
                    subnautica.SetEnabled(false);
                }
            });
        }
    }
}
