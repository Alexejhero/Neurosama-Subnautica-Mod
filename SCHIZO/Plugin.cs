global using static SCHIZO.Plugin;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Handlers;
using Nautilus.Utility;
using SCHIZO.Attributes;
using SCHIZO.Helpers;
using SCHIZO.Items;
using SCHIZO.Resources;
using SCHIZO.Unity;
using SCHIZO.Unity.Items;
using UnityEngine;

namespace SCHIZO;

[BepInPlugin("SCHIZO", "Neuro-sama Mod", "1.0.0")]
public sealed class Plugin : BaseUnityPlugin
{
#if BELOWZERO
    public static bool IS_SUBNAUTICA => false;
    public static bool IS_BELOWZERO => true;
    public static Game GAME => Game.BelowZero;
#else
    public static bool IS_SUBNAUTICA => true;
    public static bool IS_BELOWZERO => false;
    public static Game GAME => Game.Subnautica;
#endif

    public static ManualLogSource LOGGER { get; private set; }

    public static readonly Config CONFIG = OptionsPanelHandler.RegisterModOptions<Config>();

    private void Awake()
    {
        LOGGER = Logger;
        DependencyResolver.InjectResources();

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        IEnumerable<ModItem> modItems = Assets.All<ItemData>().Where(d => d.autoRegister).Select(ModItem.Create);
        modItems.ForEach(UnityPrefab.CreateAndRegister);

        // LoadMethodAttribute.LoadAll();
        // LoadComponentAttribute.AddAll(gameObject);
        // LoadConsoleCommandsAttribute.RegisterAll();
        // LoadCreatureAttribute.RegisterAll(); TODO

        /*CustomPrefab prefab = new("testermshark", "Test Ermshark", "");
        prefab.SetGameObject(() =>
        {
            GameObject parent = new();
            parent.SetActive(false);
            DontDestroyOnLoad(parent);

            GameObject instance = Instantiate(Assets.WithoutEcclibraryTestVariant, parent.transform);
            instance.SetActive(false);

            PrefabUtils.AddBasicComponents(instance, prefab.Info.ClassID, prefab.Info.TechType, LargeWorldEntity.CellLevel.Medium);

            CoroutineHelpers.RunWhen(() => MaterialHelpers.ApplySNShadersIncludingRemaps(instance, 1), () => MaterialHelpers.IsReady);

            return instance;
        });
        prefab.Register();*/
    }
}
