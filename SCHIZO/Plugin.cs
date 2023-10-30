global using static SCHIZO.Plugin;
using System.Collections;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using ECCLibrary;
using HarmonyLib;
using Nautilus.Handlers;
using SCHIZO.Attributes.Loading;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO;

[BepInPlugin("SCHIZO", "SCHIZO", "1.0.0")]
public sealed class Plugin : BaseUnityPlugin
{
    public static Assembly PLUGIN_ASSEMBLY { get; private set; }
    public static GameObject PLUGIN_OBJECT { get; private set; }
    public static ManualLogSource LOGGER { get; private set; }
    public static Harmony HARMONY { get; private set; }

    public static readonly Config CONFIG = OptionsPanelHandler.RegisterModOptions<Config>();

    private void Awake()
    {
        PLUGIN_ASSEMBLY = Assembly.GetExecutingAssembly();
        PLUGIN_OBJECT = gameObject;
        LOGGER = Logger;
        HARMONY = new Harmony("SCHIZO");

        ResourceManager.InjectAssemblies();

        SoundConfig.Provider = CONFIG;
    }

    private IEnumerator Start()
    {
        yield return ObjectReferences.SetReferences();
        yield return MaterialHelpers.LoadMaterials();
        StaticHelpers.CacheAttribute.CacheAll();

        HARMONY.PatchAll();

        Assets.Registry.InvokeRegister();
        Assets.Registry.InvokePostRegister();

        AddComponentAttribute.AddAll(gameObject);
        LoadMethodAttribute.LoadAll();

        LoadConsoleCommandsAttribute.RegisterAll();
    }
}
