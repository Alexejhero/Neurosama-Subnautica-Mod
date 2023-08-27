using System;
using System.Collections;
using System.IO;
using System.Reflection;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using Newtonsoft.Json;
using UnityEngine;

namespace SCHIZO
{
    public static class HullPlates
    {
        private static readonly DirectoryInfo HullPlateFolder = Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "HullPlates"));

        public static void Load()
        {
            foreach (string text in Directory.GetDirectories(HullPlateFolder.FullName))
            {
                string text2 = Path.Combine(text, "info.json");
                string text3 = Path.Combine(text, "icon.png");
                string text4 = Path.Combine(text, "texture.png");
                string text5 = Path.Combine(text, "override-icon.png");
                if (File.Exists(text2) && File.Exists(text3) && File.Exists(text4))
                {
                    try
                    {
                        HullPlateInfo hullPlateInfo;
                        using (StreamReader streamReader = new StreamReader(text2))
                        {
                            hullPlateInfo = new JsonSerializer().Deserialize(streamReader, typeof(HullPlateInfo)) as HullPlateInfo;
                        }
                        Texture2D icon = ImageUtils.LoadTextureFromFile(text3);
                        Texture2D texture = ImageUtils.LoadTextureFromFile(text4);
                        if (hullPlateInfo != null && icon != null && texture != null)
                        {
                            Texture2D iconTexture = texture;
                            if (File.Exists(text5))
                            {
                                iconTexture = ImageUtils.LoadTextureFromFile(text5);
                            }
                            Texture2D newIcon = TextureUtils.CombineTextures(icon, iconTexture);
                            PatchHullPlate(hullPlateInfo.InternalName, hullPlateInfo.DisplayName, hullPlateInfo.Description, newIcon, texture);
                        }
                        else
                        {
                            Console.WriteLine(string.Concat(new[]
                            {
                                "[SCHIZO] Unable to load CHP from ",
                                Path.GetDirectoryName(text),
                                "!"
                            }));
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(string.Concat(new[]
                        {
                            "[SCHIZO] Unable to load CHP from ",
                            Path.GetDirectoryName(text),
                            "!"
                        }));
                    }
                }
            }

        }

        private static void PatchHullPlate(string id, string displayName, string description, Texture2D icon, Texture2D texture)
        {
            Func<IOut<GameObject>, IEnumerator> GetPrefab()
            {
                IEnumerator GetGameObjectAsync(IOut<GameObject> gameObject)
                {
                    CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.DioramaHullPlate);
                    yield return task;

                    GameObject gameObject2 = GameObject.Instantiate(task.GetResult());
                    var dc = gameObject2.AddComponent<DontCheat>();
                    var mesh = gameObject2.FindChild("Icon").GetComponent<MeshRenderer>();
                    mesh.material.mainTexture = texture;
                    mesh.enabled = false;
                    dc.rend = mesh;
                    gameObject2.name = id;
                    gameObject.Set(gameObject2);
                }

                return GetGameObjectAsync;
            }

            var hullplate = new CustomPrefab(id, displayName, description);

            hullplate.SetGameObject(GetPrefab());
            hullplate.Info.WithIcon(ImageUtils.LoadSpriteFromTexture(icon));

            hullplate.SetPdaGroupCategory(TechGroup.Miscellaneous, TechCategory.MiscHullplates);
            if (!displayName.Contains("300 ELO"))
                hullplate.SetRecipe(new RecipeData(new CraftData.Ingredient(TechType.Titanium), new CraftData.Ingredient(TechType.Glass)));
            else
                hullplate.SetRecipe(new RecipeData(new CraftData.Ingredient(TechType.TitaniumIngot), new CraftData.Ingredient(TechType.Glass)));
            hullplate.Register();
        }

        public class HullPlateInfo
        {
            public string InternalName { get; set; }

            public string DisplayName { get; set; }

            public string Description { get; set; }

            public HullPlateInfo()
            {
            }
        }

        public class DontCheat : MonoBehaviour
        {
            private Constructable cons;
            public MeshRenderer rend;

            public void Awake()
            {
                cons = GetComponentInChildren<Constructable>();
            }

            public void Update()
            {
                if (rend) rend.enabled = cons.constructed;
            }
        }
    }
}
