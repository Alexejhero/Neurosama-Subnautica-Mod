using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using JetBrains.Annotations;
using Nautilus.Commands;
using SCHIZO.Helpers;
using UnityEngine;
using Object = UnityEngine.Object;

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

    public static string OnConsoleCommand_banks()
    {
        RuntimeManager.StudioSystem.getBankList(out Bank[] banks);
        LOGGER.LogMessage("Banks:\n" + string.Join("\n", banks.Select(b =>
        {
            b.getPath(out string bankPath);
            b.getID(out Guid bankId);
            return $"{bankId} | {bankPath}";
        })));
        return "Logged all banks, check console";
    }

    public static string OnConsoleCommand_buses()
    {
        // adapted from https://discord.com/channels/324207629784186882/324207629784186882/1065010826571956294
        RuntimeManager.StudioSystem.getBankList(out Bank[] banks);
        StringBuilder sb = new("FMOD bus list:\n");
        foreach (Bank bank in banks)
        {
            bank.getPath(out string bankPath);
            bank.getBusList(out Bus[] busArray);
            if (busArray.Length == 0) continue;
            sb.AppendLine($"Buses in bank \"{bankPath}\"");
            foreach (Bus bus in busArray)
            {
                bus.getPath(out string busPath);
                bus.getID(out Guid busId);
                sb.AppendLine($"{busId} | {busPath}");
            }
        }
        LOGGER.LogMessage(sb.ToString());
        return "Logged all buses, check console";
    }

    public static string OnConsoleCommand_vcas()
    {
        RuntimeManager.StudioSystem.getBankList(out Bank[] banks);
        StringBuilder sb = new("VCAs:\n");
        foreach (Bank bank in banks)
        {
            bank.getPath(out string bankPath);
            bank.getVCAList(out VCA[] vcaArray);
            if (vcaArray.Length == 0) continue;
            sb.AppendLine($"VCAs in bank \"{bankPath}\"");
            foreach (VCA vca in vcaArray)
            {
                vca.getPath(out string vcaPath);
                vca.getID(out Guid vcaId);
                sb.AppendLine($"{vcaId} | {vcaPath}");
            }
        }
        LOGGER.LogMessage(sb.ToString());
        return "Logged all VCAs, check console";
    }

    public static string OnConsoleCommand_events()
    {
        RuntimeManager.StudioSystem.getBankList(out Bank[] banks);
        StringBuilder sb = new("Events:\n");
        foreach (Bank bank in banks)
        {
            bank.getPath(out string bankPath);
            bank.getEventList(out EventDescription[] eventArray);
            if (eventArray.Length == 0) continue;
            sb.AppendLine($"Events in bank \"{bankPath}\"");
            foreach (EventDescription eventDesc in eventArray)
            {
                eventDesc.getPath(out string eventPath);
                eventDesc.getID(out Guid eventId);
                sb.AppendLine($"{eventId} | {eventPath}");
            }
        }
        LOGGER.LogMessage(sb.ToString());
        return "Logged all events, check console";
    }

    [ConsoleCommand("fmod"), UsedImplicitly]
    public static string OnConsoleCommand_fmod(params string[] args)
    {
        switch (args)
        {
            case ["banks"]:
                return OnConsoleCommand_banks();
            case ["buses"]:
                return OnConsoleCommand_buses();
            case ["vcas"]:
                return OnConsoleCommand_vcas();
            case ["events"]:
                return OnConsoleCommand_events();
            case ["path", string guid]:
                RuntimeManager.StudioSystem.lookupPath(Guid.Parse(guid), out string pathFromId);
                return pathFromId;
            case ["id", string path]:
                RuntimeManager.StudioSystem.lookupID(path, out Guid guidFromPath);
                return guidFromPath.ToString();
            case ["bufsize"]:
                RuntimeManager.CoreSystem.getStreamBufferSize(out uint size, out TIMEUNIT unit);
                return $"{size} {unit}";
        }

        return null;
    }
}
