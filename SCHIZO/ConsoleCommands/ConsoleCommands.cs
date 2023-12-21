using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMOD.Studio;
using FMODUnity;
using JetBrains.Annotations;
using Nautilus.Commands;
using Nautilus.Handlers;
using SCHIZO.Helpers;
using UnityEngine;
using Object = UnityEngine.Object;
using TrackId = global::Jukebox.UnlockableTrack;

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
#region FMOD
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

    public static string OnConsoleCommand_buses(string bankFilter = "")
    {
        // adapted from https://discord.com/channels/324207629784186882/324207629784186882/1065010826571956294
        RuntimeManager.StudioSystem.getBankList(out Bank[] banks);
        IEnumerable<Bank> filteredBanks = string.IsNullOrEmpty(bankFilter)
           ? banks
           : banks.Where(b =>
           {
               b.getPath(out string path);
               b.getID(out Guid id);
               return path.Contains(bankFilter) || id.ToString().Contains(bankFilter);
           });
        StringBuilder sb = new("FMOD bus list:\n");
        foreach (Bank bank in filteredBanks)
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

    public static string OnConsoleCommand_vcas(string bankFilter = "")
    {
        RuntimeManager.StudioSystem.getBankList(out Bank[] banks);
        IEnumerable<Bank> filteredBanks = string.IsNullOrEmpty(bankFilter)
            ? banks
            : banks.Where(b =>
        {
            b.getPath(out string path);
            b.getID(out Guid id);
            return path.Contains(bankFilter) || id.ToString().Contains(bankFilter);
        });
        StringBuilder sb = new("VCAs:\n");
        foreach (Bank bank in filteredBanks)
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

    public static string OnConsoleCommand_events(string bankFilter = "")
    {
        RuntimeManager.StudioSystem.getBankList(out Bank[] banks);
        IEnumerable<Bank> filteredBanks = string.IsNullOrEmpty(bankFilter)
            ? banks
            : banks.Where(b =>
            {
                b.getPath(out string path);
                b.getID(out Guid id);
                return path.Contains(bankFilter) || id.ToString().Contains(bankFilter);
            });
        StringBuilder sb = new("Events:\n");
        foreach (Bank bank in filteredBanks)
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
        return args switch
        {
            ["banks", ..] => OnConsoleCommand_banks(),
            ["buses", ..] => OnConsoleCommand_buses(args.ElementAtOrDefault(1)),
            ["vcas", ..] => OnConsoleCommand_vcas(args.ElementAtOrDefault(1)),
            ["events", ..] => OnConsoleCommand_events(args.ElementAtOrDefault(1)),
            ["path", string guid] => FMODHelpers.GetPath(guid),
            ["id", string path] => FMODHelpers.GetId(path),
            _ => null,
        };
    }
    #endregion FMOD

    [ConsoleCommand("unlocktrack"), UsedImplicitly]
    public static string OnConsoleCommand_unlocktrack(params string[] trackArgs)
    {
        string track = string.Join(" ", trackArgs);
        if (string.IsNullOrEmpty(track))
            return "Usage: unlocktrack <track id>\nTrack ID can be specified by enum name or value (e.g. `Track1` or just `1`)";
        // Nautilus only patches Enum.Parse...
        if (!Enum.TryParse(track, true, out TrackId trackId) && !EnumHandler.TryGetValue(track, out trackId))
            return $"No such track '{track}'";

        if (!Player.main) return "Jukebox tracks can only be unlocked in-game";
        global::Jukebox.Unlock(trackId, true);
        return null;
    }
}
