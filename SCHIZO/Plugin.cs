using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using Nautilus.Handlers;
using SCHIZO.Attributes;
using SCHIZO.Resources;
using SCHIZO.Unity;
using SCHIZO.Unity.Materials;

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

        LoadMethodAttribute.LoadAll();
        LoadComponentAttribute.AddAll(gameObject);
        LoadConsoleCommandsAttribute.RegisterAll();
        LoadCreatureAttribute.RegisterAll();
    }
}
