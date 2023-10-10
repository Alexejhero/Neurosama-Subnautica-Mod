using System.Collections.Generic;
using NaughtyAttributes.Editor;
using PropertyDrawers;
using SCHIZO.Unity.Items;
using UnityEditor;

namespace Inspectors
{
    [CustomEditor(typeof(ItemData))]
    public sealed class ItemDataInspector : NaughtyInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ItemData itemData = (ItemData) target;
            RecipeDrawer.SubnauticaRecipes = new List<int>() {itemData.RecipeSN.GetInstanceID()};
            RecipeDrawer.BelowZeroRecipes = new List<int>() {itemData.RecipeBZ.GetInstanceID()};
            RecipeDrawer.BuildableRecipes = itemData.isBuildable ? new List<int>() {itemData.RecipeSN.GetInstanceID(), itemData.RecipeBZ.GetInstanceID()} : new List<int>();
            RecipeDrawer.NonBuildableRecipes = !itemData.isBuildable ? new List<int>() {itemData.RecipeSN.GetInstanceID(), itemData.RecipeBZ.GetInstanceID()} : new List<int>();
        }

        private new void OnEnable()
        {
            base.OnEnable();

            ItemData itemData = (ItemData) target;
            RecipeDrawer.SubnauticaRecipes = new List<int>() {itemData.RecipeSN.GetInstanceID()};
            RecipeDrawer.BelowZeroRecipes = new List<int>() {itemData.RecipeBZ.GetInstanceID()};
            RecipeDrawer.BuildableRecipes = itemData.isBuildable ? new List<int>() {itemData.RecipeSN.GetInstanceID(), itemData.RecipeBZ.GetInstanceID()} : new List<int>();
            RecipeDrawer.NonBuildableRecipes = !itemData.isBuildable ? new List<int>() {itemData.RecipeSN.GetInstanceID(), itemData.RecipeBZ.GetInstanceID()} : new List<int>();
        }

        private new void OnDisable()
        {
            base.OnDisable();

            RecipeDrawer.SubnauticaRecipes.Clear();
            RecipeDrawer.BelowZeroRecipes.Clear();
            RecipeDrawer.BuildableRecipes.Clear();
            RecipeDrawer.NonBuildableRecipes.Clear();
        }
    }
}
