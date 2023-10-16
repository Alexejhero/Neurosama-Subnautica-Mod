using System;
using System.Collections.Generic;
using NaughtyAttributes;
using SCHIZO.Items.Data.Crafting;
using SCHIZO.Registering;
using UnityEngine;

namespace SCHIZO.HullPlates
{
    [CreateAssetMenu(menuName = "SCHIZO/Hull Plates/Hull Plate Loader")]
    public sealed partial class HullPlateLoader : ModRegistryItem
    {
        [Required] public Texture2D hiddenIcon;
        [Required] public Texture2D deprecatedTexture;
        [Required] public Recipe recipeRegular;
        [Required] public Recipe recipeExpensive;
        [ReorderableList] public List<HullPlate> hullPlates;

        [Button]
        private void Sort()
        {
            hullPlates.Sort((a, b) =>
            {
                return a.deprecated.CompareTo(b.deprecated) * 2 +
                       Math.Sign(string.Compare(a.name, b.name, StringComparison.Ordinal));
            });
        }
    }
}
