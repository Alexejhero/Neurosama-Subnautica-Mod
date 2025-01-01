using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Input;
using SCHIZO.Helpers;
using SCHIZO.Tweaks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SCHIZO.Commands;

[CommandCategory("Developer")]
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
    public static void Say([TakeRemaining] string message = "")
    {
        ErrorMessage.AddMessage(message);
    }

    [Command(Name = "hint",
        DisplayName = "Message (hint)",
        Description = "Display a hint message",
        RegisterConsoleCommand = true)]
    public static void Hint([TakeRemaining] string message = "")
    {
        if (string.IsNullOrEmpty(message))
            return;
        MessageHelpers.ShowHint(5, message);
    }

    [Command(Name = "resetcam",
        DisplayName = "Reset camera",
        Description = "Reset camera position and rotation (for debugging)",
        RegisterConsoleCommand = true)]
    public static void ResetCam()
    {
        if (!SNCameraRoot.main) return;

        SNCameraRoot.main.transform.localPosition = Vector3.zero;
        SNCameraRoot.main.transform.localRotation = Quaternion.identity;
    }

    [Command(Name = "duh",
        DisplayName = "Dev mode",
        Description = "Toggle dev mode",
        RegisterConsoleCommand = true)]
    public static void DevMode()
    {
        if (!Player.main) return;

        DevConsole.SendConsoleCommand("fastswim");

        DevBinds binds = Player.main.gameObject.GetComponent<DevBinds>();
        if (binds)
            binds.enabled = !binds.enabled;
        else
            Player.main.gameObject.AddComponent<DevBinds>();
    }

    [Command(Name = "_get",
        DisplayName = "Get static member",
        Description = "Get value of a static member",
        RegisterConsoleCommand = true)]
    public static object PrintField(string typeName, string memberName)
    {
        Type type = ReflectionCache.GetType(typeName);
        if (type is null) return $"Could not find type '{typeName}'";
        MemberInfo[] member = type.GetMember(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        if (member.Length == 0) return $"Could not find '{memberName}' in type '{typeName}' (must be static)";
        return ReflectionHelpers.GetStaticMemberValue<object>(member.Single());
    }

    [Command(Name = "_set",
        DisplayName = "Set static member",
        Description = "Set value of a static member",
        RegisterConsoleCommand = true)]
    public static object SetField(string typeName, string memberName, string valueStr)
    {
        Type type = ReflectionCache.GetType(typeName);
        if (type is null) return $"Could not find type '{typeName}'";
        MemberInfo member = type.GetMember(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).SingleOrDefault();
        if (member is null) return $"Could not find '{memberName}' in type '{typeName}' (must be static)";
        Type memberType = member switch
        {
            PropertyInfo p => p.PropertyType,
            FieldInfo f => f.FieldType,
            _ => null
        };
        if (memberType is null) return $"'{memberName}' is not a property or field";

        if (!Conversion.TryParseOrConvert(valueStr, memberType, out object value))
        {
            return $"Could not convert '{valueStr}' to type '{memberType}'";
        }
        switch (member)
        {
            case PropertyInfo prop:
                prop.SetValue(null, value);
                return null;
            case FieldInfo field:
                field.SetValue(null, value);
                return null;
            default:
                throw new InvalidOperationException("Unreachable");
        }
    }


    [Command(Name = "dump_spawn_biomes",
        DisplayName = "Dump Spawn Biomes",
        Description = "List the biomes where a given techtype is registered to spawn",
        RegisterConsoleCommand = true)]
    public static string DumpSpawnBiomes(string classIdOrTechType)
    {
        string classId = Guid.TryParse(classIdOrTechType, out Guid guid) ? guid.ToString()
            : UWE.Utils.TryParseEnum(classIdOrTechType, out TechType techType) ? CraftData.GetClassIdForTechType(techType)
            : null;

        if (classId is null) return MessageHelpers.TechTypeNotFound(classIdOrTechType);

        CSVEntitySpawner spawner = GameObject.FindObjectOfType<CSVEntitySpawner>();
        LootDistributionData lootData = spawner ? spawner.lootDistribution : BiomeHelpers.baseGameLootData;

        return lootData.srcDistribution.TryGetValue(classId, out LootDistributionData.SrcData srcData)
            ? string.Join(",", srcData.distribution.Select(bd => bd.biome))
            : $"No data for {classIdOrTechType}";
    }
}
