using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMOD.Studio;
using FMODUnity;
using JetBrains.Annotations;
using Nautilus.Commands;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.ConsoleCommands;

[RegisterConsoleCommands]
public static class ConsoleCommands
{
    [ConsoleCommand("isekai"), UsedImplicitly]
    public static string OnConsoleCommand_isekai(string techTypeName, float percentage, float radius = 100)
    {
        if (!UWE.Utils.TryParseEnum(techTypeName, out TechType techType))
        {
            IEnumerable<string> techTypeNamesSuggestion = TechTypeExtensions.GetTechTypeNamesSuggestion(techTypeName);
            return MessageHelpers.GetCommandOutput($"Could not find tech type for '{techTypeName}'. Did you mean:\n{string.Join("\n", techTypeNamesSuggestion)}");
        }

        List<PrefabIdentifier> items = PhysicsHelpers.ObjectsInRange(Player.main.transform, radius)
            .OfTechType(techType).Where(g => g.GetComponentInParent<Pickupable>() is not { inventoryItem: { } })
            .SelectComponentInParent<PrefabIdentifier>().ToList();
        items.Shuffle();
        HashSet<PrefabIdentifier> set = [..items];

        foreach (PrefabIdentifier item in set.Take((int) Mathf.Round(set.Count * percentage)))
        {
            Object.Destroy(item.gameObject);
        }

        return null;
    }

    [ConsoleCommand("say"), UsedImplicitly]
    public static void OnConsoleCommand_say(params string[] args)
    {
        // wishlist todo: make nautilus pass the whole command string
        // if there's a single string parameter named something like __all
        // so we don't have to needlessly split and join
        ErrorMessage.AddMessage(string.Join(" ", args));
    }

    [ConsoleCommand("buses"), UsedImplicitly]
    public static string OnConsoleCommand_buses()
    {
        // adapted from https://discord.com/channels/324207629784186882/324207629784186882/1065010826571956294
        RuntimeManager.StudioSystem.getBankList(out Bank[] banks);
        StringBuilder sb = new("FMOD bus list:\n");
        foreach (Bank bank in banks)
        {
            bank.getPath(out string bankPath);
            bank.getBusList(out Bus[] busArray);
            foreach (Bus bus in busArray)
            {
                bus.getPath(out string busPath);
                sb.AppendLine($"bankPath: {bankPath}; busPath: {busPath}");
            }
        }
        LOGGER.LogMessage(sb.ToString());
        return "Logged all buses, check console";
    }
}
