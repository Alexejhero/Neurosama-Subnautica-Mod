using System.Collections.Generic;
using PropertyDrawers;
using SCHIZO.Items.Data;
using UnityEditor;

namespace Inspectors
{
    [CustomEditor(typeof(ItemData))]
    public sealed class ItemDataInspector : ModRegistryItemInspector
    {
        private void SetFields()
        {
            ItemData itemData = (ItemData) target;

            int sn = itemData.RecipeSN ? itemData.RecipeSN.GetInstanceID() : 0;
            int bz = itemData.RecipeBZ ? itemData.RecipeBZ.GetInstanceID() : 0;

            RecipeDrawer.SubnauticaRecipes = new List<int>() {sn};
            RecipeDrawer.BelowZeroRecipes = new List<int>() {bz};
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            SetFields();
        }

        private new void OnEnable()
        {
            base.OnEnable();
            SetFields();
        }

        private new void OnDisable()
        {
            base.OnDisable();

            RecipeDrawer.SubnauticaRecipes.Clear();
            RecipeDrawer.BelowZeroRecipes.Clear();
        }
    }
}
