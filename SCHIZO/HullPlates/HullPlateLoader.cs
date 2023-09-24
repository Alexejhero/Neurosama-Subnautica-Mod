using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using SCHIZO.Attributes;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using SCHIZO.Unity.HullPlates;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SCHIZO.HullPlates;

[LoadMethod]
[SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
public static class HullPlateLoader
{
    private static readonly RecipeData NormalRecipe = new(new Ingredient(TechType.Titanium, 1), new Ingredient(TechType.Glass, 1));
    private static readonly RecipeData ExpensiveRecipe = new(new Ingredient(TechType.TitaniumIngot, 1), new Ingredient(TechType.Glass, 1));

    [LoadMethod]
    private static void Load()
    {
        HullPlateCollection collection = ResourceManager.LoadAsset<HullPlateCollection>("Hull Plates");

        foreach (HullPlate hullPlate in collection.hullPlates)
        {
            if (hullPlate.deprecated) throw new Exception("Deprecated hull plate found in regular hull plate list!");
            LoadHullPlate(hullPlate, collection.hiddenIcon);
        }

        foreach (HullPlate hullPlate in collection.deprecatedHullPlates)
        {
            if (!hullPlate.deprecated) throw new Exception("Regular hull plate found in deprecated hull plate list!");
            LoadOldHullPlate(hullPlate, collection.deprecatedTexture);
        }
    }

    private static void LoadHullPlate(HullPlate hullPlate, Texture2D hiddenIcon)
    {
        Texture2D overrideIcon = hullPlate.overrideIcon !?? hullPlate.texture;
        overrideIcon = overrideIcon.Scale(177, 176).Translate(-21, -38).Crop(hiddenIcon.width, hiddenIcon.height);

        Texture2D newIcon = hullPlate.hidden ? hiddenIcon : TextureHelpers.BlendAlpha(hiddenIcon, overrideIcon);

        CustomPrefab hullplate = new(hullPlate.classId, hullPlate.displayName, hullPlate.tooltip);
        hullplate.SetGameObject(GetPrefab(hullPlate.texture, hullPlate.classId));
        hullplate.Info.WithIcon(ImageUtils.LoadSpriteFromTexture(newIcon));
        hullplate.SetPdaGroupCategory(TechGroup.Miscellaneous, TechCategory.MiscHullplates);
        hullplate.SetRecipe(hullPlate.expensive ? ExpensiveRecipe : NormalRecipe);
        hullplate.Register();
    }

    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
    private static void LoadOldHullPlate(HullPlate hullPlate, Texture2D deprecatedTexture)
    {
        CustomPrefab hullplate = new(hullPlate.classId, hullPlate.displayName + " (REMOVED FROM MOD)", "");
        hullplate.SetGameObject(GetPrefab(deprecatedTexture, hullPlate.classId));
        hullplate.SetRecipe(hullPlate.expensive ? ExpensiveRecipe : NormalRecipe);
        hullplate.Register();
    }

    private static Func<IOut<GameObject>, IEnumerator> GetPrefab(Texture2D texture, string id)
    {
        return GetPrefabAsync;

        IEnumerator GetPrefabAsync(IOut<GameObject> gameObject)
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.DioramaHullPlate);
            yield return task;

            GameObject instance = Object.Instantiate(task.GetResult());
            TextureHider hider = instance.AddComponent<TextureHider>();
            MeshRenderer mesh = instance.FindChild("Icon").GetComponent<MeshRenderer>();
            mesh.material.mainTexture = texture;
            mesh.enabled = false;
            hider.rend = mesh;
            instance.name = id;
            gameObject.Set(instance);
        }
    }
}
