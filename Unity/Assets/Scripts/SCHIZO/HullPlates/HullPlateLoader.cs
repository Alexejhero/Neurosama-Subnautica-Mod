using System;
using System.Collections.Generic;
using TriInspector;
using SCHIZO.Items.Data.Crafting;
using SCHIZO.Registering;
using UnityEngine;

namespace SCHIZO.HullPlates
{
    [CreateAssetMenu(menuName = "SCHIZO/Hull Plates/Hull Plate Loader")]
    public sealed partial class HullPlateLoader : ModRegistryItem
    {
        [Required] public Texture2D hiddenIcon;
        [Required] public Texture2D missingTexture;
        [Required] public Recipe recipeRegular;
        [Required] public Recipe recipeExpensive;
        [ListDrawerSettings(AlwaysExpanded = true)] public List<HullPlate> hullPlates;

        [Button]
        private void Sort()
        {
            hullPlates.Sort((a, b) =>
            {
                int deprecated = a.deprecated.CompareTo(b.deprecated);
                if (deprecated != 0) return deprecated;
                return Math.Sign(string.CompareOrdinal(a.name, b.name));
            });
        }
    }
}
