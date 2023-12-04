global using static SCHIZO.Plugin;
using System.Collections;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using ECCLibrary;
using FMOD;
using FMODUnity;
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
        byte[] fmodBank = ResourceManager.GetEmbeddedBytes("SCHIZO.bank", true);
        RESULT res = RuntimeManager.StudioSystem.loadBankMemory(fmodBank, FMOD.Studio.LOAD_BANK_FLAGS.NORMAL, out FMOD.Studio.Bank bank);
        res.CheckResult();
        if (!bank.hasHandle())
        {
            LOGGER.LogError($"Could not load FMOD bank");
            throw new BankLoadException("SCHIZO.bank", res);
        }

        HARMONY.PatchAll();
    }

    private IEnumerator Start()
    {
        yield return ObjectReferences.SetReferences();
        yield return MaterialHelpers.LoadMaterials();
        StaticHelpers.CacheAttribute.CacheAll();

        Assets.Mod_Registry.InvokeRegister();
        Assets.Mod_Registry.InvokePostRegister();

        RegisterConsoleCommandsAttribute.RegisterAll();
    }
}
