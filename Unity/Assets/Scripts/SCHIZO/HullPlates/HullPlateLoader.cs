using System;
using System.Collections.Generic;
using TriInspector;
using SCHIZO.Items.Data.Crafting;
using SCHIZO.Registering;
using UnityEngine;
using UnityEngine.Serialization;

namespace SCHIZO.HullPlates
{
    [CreateAssetMenu(menuName = "SCHIZO/Hull Plates/Hull Plate Loader")]
    public sealed partial class HullPlateLoader : ModRegistryItem
    {
        [Required] public Texture2D hiddenIcon;
        [FormerlySerializedAs("deprecatedTexture"), Required] public Texture2D missingTexture;
        [Required] public Recipe recipeRegular;
        [Required] public Recipe recipeExpensive;
        [ListDrawerSettings] public List<HullPlate> hullPlates;

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
