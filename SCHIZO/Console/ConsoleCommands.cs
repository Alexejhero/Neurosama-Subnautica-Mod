using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMOD.Studio;
using FMODUnity;
using JetBrains.Annotations;
using Nautilus.Commands;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Console;

[LoadConsoleCommands]
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
        HashSet<PrefabIdentifier> set = new(items);

        foreach (PrefabIdentifier item in set.Take((int) Mathf.Round(set.Count * percentage)))
        {
            Object.Destroy(item.gameObject);
        }

        return null;
    }

    [ConsoleCommand("say"), UsedImplicitly]
    public static void OnConsoleCommand_say(
        string  _1 = "", string  _2 = "", string  _3 = "", string  _4 = "", string  _5 = "",
        string  _6 = "", string  _7 = "", string  _8 = "", string  _9 = "", string _10 = "",
        string _11 = "", string _12 = "", string _13 = "", string _14 = "", string _15 = "",
        string _16 = "", string _17 = "", string _18 = "", string _19 = "", string _20 = "",
        string _21 = "", string _22 = "", string _23 = "", string _24 = "", string _25 = "",
        string _26 = "", string _27 = "", string _28 = "", string _29 = "", string _30 = "",
        string _31 = "", string _32 = "", string _33 = "", string _34 = "", string _35 = "",
        string _36 = "", string _37 = "", string _38 = "", string _39 = "", string _40 = "",
        string _41 = "", string _42 = "", string _43 = "", string _44 = "", string _45 = "",
        string _46 = "", string _47 = "", string _48 = "", string _49 = "", string _50 = "")
    {
        ErrorMessage.AddMessage(string.Join(" ",
             _1,  _2,  _3,  _4,  _5,  _6,  _7,  _8,  _9, _10,
            _11, _12, _13, _14, _15, _16, _17, _18, _19, _20,
            _21, _22, _23, _24, _25, _26, _27, _28, _29, _30,
            _31, _32, _33, _34, _35, _36, _37, _38, _39, _40,
            _41, _42, _43, _44, _45, _46, _47, _48, _49, _50));
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
