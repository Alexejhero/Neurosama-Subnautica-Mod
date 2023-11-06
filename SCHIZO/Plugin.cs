global using static SCHIZO.Plugin;
using System.Collections;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using ECCLibrary;
using HarmonyLib;
using SCHIZO.ConsoleCommands;
using SCHIZO.Helpers;
using SCHIZO.Resources;
using UnityEngine;

namespace SCHIZO;

[BepInPlugin("SCHIZO", "SCHIZO", "1.0.0")]
public sealed class Plugin : BaseUnityPlugin
{
    public static Assembly PLUGIN_ASSEMBLY { get; private set; }
    public static GameObject PLUGIN_OBJECT { get; private set; }
    public static ManualLogSource LOGGER { get; private set; }
    public static Harmony HARMONY { get; private set; }

    private void Awake()
    {
        PLUGIN_ASSEMBLY = Assembly.GetExecutingAssembly();
        PLUGIN_OBJECT = gameObject;
        LOGGER = Logger;
        HARMONY = new Harmony("SCHIZO");

        ResourceManager.InjectAssemblies();
    }

    private IEnumerator Start()
    {
        yield return ObjectReferences.SetReferences();
        yield return MaterialHelpers.LoadMaterials();
        StaticHelpers.CacheAttribute.CacheAll();

        HARMONY.PatchAll();

        Assets.Mod_Registry.InvokeRegister();
        Assets.Mod_Registry.InvokePostRegister();

        RegisterConsoleCommandsAttribute.RegisterAll();
    }
}
