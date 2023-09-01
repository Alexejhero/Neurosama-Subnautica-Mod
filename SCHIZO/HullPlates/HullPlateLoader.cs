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
    private static readonly DirectoryInfo HullPlateFolder = Directory.CreateDirectory(Path.Combine(AssetLoader.AssetsFolder, "hullplates"));

    public static void Load()
    {
        foreach (string path in Directory.GetDirectories(HullPlateFolder.FullName))
        {
            LoadHullPlate(path);
        }
    }

    private static void LoadHullPlate(string path)
    {
        string infoPath = Path.Combine(path, "info.json");
        string iconPath = Path.Combine(path, "icon.png");
        string texturePath = Path.Combine(path, "texture.png");
        string spriteOverridePath = Path.Combine(path, "override-icon.png");
        if (!File.Exists(infoPath) || !File.Exists(iconPath) || !File.Exists(texturePath)) return;

        using StreamReader streamReader = new(infoPath);
        HullPlateInfo hullPlateInfo = (HullPlateInfo) new JsonSerializer().Deserialize(streamReader, typeof(HullPlateInfo));

        Texture2D icon = ImageUtils.LoadTextureFromFile(iconPath);
        Texture2D texture = ImageUtils.LoadTextureFromFile(texturePath);
        Texture2D spriteOverride = texture;
        if (File.Exists(spriteOverridePath))
        {
            spriteOverride = ImageUtils.LoadTextureFromFile(spriteOverridePath);
        }

        Texture2D newIcon = TextureHelpers.BlendAlpha(icon, spriteOverride);
        PatchHullPlate(hullPlateInfo!.InternalName, hullPlateInfo.DisplayName, hullPlateInfo.Description, newIcon, texture, hullPlateInfo.Expensive);
    }

    private static void PatchHullPlate(string id, string displayName, string description, Texture2D icon, Texture2D texture, bool expensive)
    {
        CustomPrefab hullplate = new(id, displayName, description);

        hullplate.SetGameObject(GetPrefab(texture, id));
        hullplate.Info.WithIcon(ImageUtils.LoadSpriteFromTexture(icon));

        hullplate.SetPdaGroupCategory(TechGroup.Miscellaneous, TechCategory.MiscHullplates);
        hullplate.SetRecipe(new RecipeData(new CraftData.Ingredient(!expensive ? TechType.Titanium : TechType.TitaniumIngot), new CraftData.Ingredient(TechType.Glass)));
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
