global using static SCHIZO.Plugin;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using SCHIZO.Attributes.Loading;
using SCHIZO.Resources;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO;

[BepInPlugin("SCHIZO", "SCHIZO", "1.0.0")]
public sealed class Plugin : BaseUnityPlugin
{
    public static GameObject PLUGIN_OBJECT { get; private set; }
    public static ManualLogSource LOGGER { get; private set; }
    public static Harmony HARMONY { get; private set; }

    public static readonly Config CONFIG = OptionsPanelHandler.RegisterModOptions<Config>();

    private void Awake()
    {
        PLUGIN_OBJECT = gameObject;
        LOGGER = Logger;
        HARMONY = new Harmony("SCHIZO");

        ResourceManager.InjectAssemblies();
        SoundConfig.Provider = CONFIG;
        HARMONY.PatchAll();

        Assets.Registry.InvokeRegister();
        Assets.Registry.InvokePostRegister();

        AddComponentAttribute.AddAll(gameObject);
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
