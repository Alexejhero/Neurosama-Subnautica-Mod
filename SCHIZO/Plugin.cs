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

        HARMONY.PatchAll();
    }

    private void LoadBank(string file, bool loadStrings = true)
    {
        byte[] fmodBank = ResourceManager.GetEmbeddedBytes($"{file}.bank", true);
        RESULT res = RuntimeManager.StudioSystem.loadBankMemory(fmodBank, FMOD.Studio.LOAD_BANK_FLAGS.NORMAL, out FMOD.Studio.Bank bank);
        res.CheckResult();
        if (!bank.hasHandle())
        {
            throw new BankLoadException(file, res);
        }
        if (loadStrings) LoadBank($"{file}.strings", false);
    }

    private IEnumerator Start()
    {
        yield return ObjectReferences.SetReferences();
        yield return MaterialHelpers.LoadMaterials();
        StaticHelpers.CacheAttribute.CacheAll();

        try
        {
            LoadBank("SCHIZO");
        }
        catch (BankLoadException e)
        {
            LOGGER.LogError($"Could not load FMOD bank {e}");
        }

        Assets.Mod_Registry.InvokeRegister();
        Assets.Mod_Registry.InvokePostRegister();

        RegisterConsoleCommandsAttribute.RegisterAll();
    }
}
