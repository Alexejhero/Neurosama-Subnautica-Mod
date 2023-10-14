global using static SCHIZO.Plugin;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using SCHIZO.Attributes;
using SCHIZO.Items;
using SCHIZO.Resources;
using SCHIZO.Sounds;
using SCHIZO.Unity.Items;

namespace SCHIZO;

[BepInPlugin("SCHIZO", "Neuro-sama Mod", "1.0.0")]
public sealed class Plugin : BaseUnityPlugin
{
    public static ManualLogSource LOGGER { get; private set; }

    public static readonly Config CONFIG = OptionsPanelHandler.RegisterModOptions<Config>();

    private void Awake()
    {
        LOGGER = Logger;
        DependencyResolver.InjectResources();

        SoundConfig.Provider = CONFIG;

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        IEnumerable<ModItem> modItems = Assets.All<ItemData>().Where(d => d.autoRegister).Select(ModItem.Create);
        modItems.ForEach(UnityPrefab.CreateAndRegister);

        AddComponentAttribute.AddAll(gameObject, AddComponentAttribute.Target.Plugin);
        LoadMethodAttribute.LoadAll();

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
