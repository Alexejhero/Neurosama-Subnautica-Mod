using System;
using System.Collections;
using System.IO;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using Newtonsoft.Json;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.HullPlates;

public static class HullPlateLoader
{
    private static readonly Texture2D _baseIcon = AssetLoader.GetTexture("../hullplates/icon.png");
    private static readonly Texture2D _oldHullPlateTexture = AssetLoader.GetTexture("../old_hullplates/texture.png");
    private static readonly DirectoryInfo _hullPlatesFolder = Directory.CreateDirectory(Path.Combine(AssetLoader.AssetsFolder, "hullplates"));
    private static readonly DirectoryInfo _oldHullPlatesFolder = Directory.CreateDirectory(Path.Combine(AssetLoader.AssetsFolder, "old_hullplates"));

    public static void Load()
    {
        foreach (string path in Directory.GetDirectories(_hullPlatesFolder.FullName))
        {
            LoadHullPlate(path);
        }

        foreach (string path in Directory.GetDirectories(_oldHullPlatesFolder.FullName))
        {
            LoadOldHullPlate(path);
        }
    }

    private static void LoadHullPlate(string path)
    {
        string infoPath = Path.Combine(path, "info.json");
        string texturePath = Path.Combine(path, "texture.png");
        string spriteOverridePath = Path.Combine(path, "override-icon.png");
        if (!File.Exists(infoPath) || !File.Exists(texturePath)) return;

        using StreamReader streamReader = new(infoPath);
        HullPlateInfo hullPlateInfo = (HullPlateInfo) new JsonSerializer().Deserialize(streamReader, typeof(HullPlateInfo));

        Texture2D texture = ImageUtils.LoadTextureFromFile(texturePath);
        Texture2D spriteOverride = texture;
        if (File.Exists(spriteOverridePath))
        {
            spriteOverride = ImageUtils.LoadTextureFromFile(spriteOverridePath);
        }
        // definitely not fragile
        spriteOverride = spriteOverride.Scale(152, 145).Translate(-17, -28).Crop(_baseIcon.width, _baseIcon.height);

        Texture2D newIcon = hullPlateInfo!.Hidden ? _baseIcon : TextureHelpers.BlendAlpha(_baseIcon, spriteOverride);

        CustomPrefab hullplate = new(hullPlateInfo!.InternalName, hullPlateInfo.DisplayName, hullPlateInfo.Description);
        hullplate.SetGameObject(GetPrefab(texture, hullPlateInfo!.InternalName));
        hullplate.Info.WithIcon(ImageUtils.LoadSpriteFromTexture(newIcon));
        hullplate.SetPdaGroupCategory(TechGroup.Miscellaneous, TechCategory.MiscHullplates);
        hullplate.SetRecipe(new RecipeData(new CraftData.Ingredient(!hullPlateInfo.Expensive ? TechType.Titanium : TechType.TitaniumIngot), new CraftData.Ingredient(TechType.Glass)));
        hullplate.Register();
    }

    private static void LoadOldHullPlate(string path)
    {
        string infoPath = Path.Combine(path, "info.json");

        using StreamReader streamReader = new(infoPath);
        HullPlateInfo hullPlateInfo = (HullPlateInfo) new JsonSerializer().Deserialize(streamReader, typeof(HullPlateInfo));

        CustomPrefab hullplate = new(hullPlateInfo!.InternalName, hullPlateInfo.DisplayName + " (OLD, PLEASE REBUILD)", hullPlateInfo.Description);
        hullplate.SetGameObject(GetPrefab(_oldHullPlateTexture, hullPlateInfo!.InternalName));
        hullplate.SetRecipe(new RecipeData(new CraftData.Ingredient(!hullPlateInfo.Expensive ? TechType.Titanium : TechType.TitaniumIngot), new CraftData.Ingredient(TechType.Glass)));
        hullplate.Register();
    }

    private static Func<IOut<GameObject>, IEnumerator> GetPrefab(Texture2D texture, string id)
    {
        return GetPrefabAsync;

        IEnumerator GetPrefabAsync(IOut<GameObject> gameObject)
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.DioramaHullPlate);
            yield return task;

            GameObject instance = GameObject.Instantiate(task.GetResult());
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
