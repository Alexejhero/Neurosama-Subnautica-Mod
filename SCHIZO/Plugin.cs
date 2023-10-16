global using static SCHIZO.Plugin;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using SCHIZO.Attributes.Loading;
using SCHIZO.Items;
using SCHIZO.Items.Data;
using SCHIZO.Resources;
using SCHIZO.Sounds;
using ComponentAdder = SCHIZO.Registering.ComponentAdder;

namespace SCHIZO;

[BepInPlugin("SCHIZO", "Neuro-sama Mod", "1.0.0")]
public sealed class Plugin : BaseUnityPlugin
{
    public static ManualLogSource LOGGER { get; private set; }
    public static Harmony HARMONY { get; private set; }

    public static readonly Config CONFIG = OptionsPanelHandler.RegisterModOptions<Config>();

    private void Awake()
    {
        LOGGER = Logger;
        ResourceManager.InjectAssemblies();

        SoundConfig.Provider = CONFIG;

        HARMONY = new Harmony("SCHIZO");
        HARMONY.PatchAll();

        IEnumerable<ModItem> modItems = Assets.All<ItemData>().Where(d => d.autoRegister).Select(ModItem.Create);
        modItems.ForEach(UnityPrefab.CreateAndRegister);

        Assets.All<ComponentAdder>().ForEach(a => a.Patch(gameObject));

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
