using System.Collections.Generic;
using System.Linq;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Output;
using SCHIZO.Helpers;
using SCHIZO.Tweaks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SCHIZO.Commands;

[RegisterCommands("Developer")]
public static class DevCommands
{
    [Command(Name = "isekai",
        DisplayName = "Isekai",
        Description = "Send surrounding objects to another world.",
        RegisterConsoleCommand = true)]
    public static string Isekai(string techType, float proportion = 1, float radius = 100)
    {
        if (!Player.main) return null;
        if (!UWE.Utils.TryParseEnum(techType, out TechType techTypeActual))
            return MessageHelpers.TechTypeNotFound(techType);

        List<PrefabIdentifier> items = PhysicsHelpers.ObjectsInRange(Player.main.transform, radius)
            .OfTechType(techTypeActual)
            .Where(g => g && g.GetComponentInParent<Pickupable>() is not { inventoryItem: { } })
            .SelectComponentInParent<PrefabIdentifier>()
            .Distinct()
            .ToList();
        items.Shuffle();

        foreach (PrefabIdentifier item in items.Take(Mathf.RoundToInt(items.Count * proportion)))
        {
            if (!item) continue;
            Object.Destroy(item.gameObject);
        }

        return null;
    }

    [Command(Name = "say",
        DisplayName = "Message (top left)",
        Description = "Display a text message in the top left corner of the screen.",
        RegisterConsoleCommand = true)]
    public static object Say([TakeAll] string message)
    {
        if (string.IsNullOrEmpty(message))
            return CommonResults.ShowUsage();

        ErrorMessage.AddMessage(message);
        return null;
    }

    [Command(Name = "hint",
        DisplayName = "Message (hint)",
        Description = "Display a hint message",
        RegisterConsoleCommand = true)]
    public static object Hint([TakeAll] string message)
    {
        if (string.IsNullOrEmpty(message))
            return CommonResults.ShowUsage();

        MessageHelpers.ShowHint(5, message);
        return null;
    }

    [Command(Name = "resetcam",
        DisplayName = "Reset camera",
        Description = "Reset camera position and rotation (for debugging)",
        RegisterConsoleCommand = true)]
    public static void ResetCam()
    {
        SNCameraRoot.main.transform.localPosition = Vector3.zero;
        SNCameraRoot.main.transform.localRotation = Quaternion.identity;
    }

    [Command(Name = "duh",
        DisplayName = "Dev mode",
        Description = "Toggle dev mode",
        RegisterConsoleCommand = true)]
    public static void DevMode()
    {
        DevConsole.SendConsoleCommand("fastswim");

        DevBinds binds = Player.main.gameObject.GetComponent<DevBinds>();
        if (binds)
            binds.enabled = !binds.enabled;
        else
            Player.main.gameObject.AddComponent<DevBinds>();
    }
}
