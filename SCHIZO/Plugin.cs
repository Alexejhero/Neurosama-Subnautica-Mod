global using static SCHIZO.Plugin;
global using static SCHIZO.API.Global;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using SCHIZO.API;
using SCHIZO.API.Attributes;
using SCHIZO.Resources;

namespace SCHIZO;

[BepInPlugin("SCHIZO", "Neuro-sama Mod", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    public static ManualLogSource LOGGER { get; private set; }

    public static readonly Config CONFIG = OptionsPanelHandler.RegisterModOptions<Config>();

    private void Awake()
    {
        MAIN_ASSEMBLY = Assembly.GetExecutingAssembly();

        LOGGER = Logger;
        DependencyResolver.InjectResources();

        LoadMethodAttribute.LoadAll();
        LoadComponentAttribute.AddAll(gameObject);
        LoadConsoleCommandsAttribute.RegisterAll();
        LoadCreatureAttribute.RegisterAll();

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        Harmony.CreateAndPatchAll(typeof(ModItem).Assembly);
    }
}
