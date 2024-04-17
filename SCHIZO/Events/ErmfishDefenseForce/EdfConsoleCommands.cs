using System.Collections.Generic;
using Nautilus.Commands;
using SCHIZO.ConsoleCommands;

namespace SCHIZO.Events.ErmfishDefenseForce;

[RegisterConsoleCommands]
public static class EdfConsoleCommands
{
    private const string CommandName = "edf";
    [ConsoleCommand(CommandName)]
    public static string EdfCommand(params string[] args)
    {
        if (args is [])
            return "subcommands:\naggro\nreset";
        string subCommand = args[0].ToLower();
        return subCommand switch
        {
            "aggro" => Aggro(args),
            "reset" => Reset(),
            //"spawn" => ForceSpawn(args),
            _ => null,
        };
    }

    public static string Aggro(IReadOnlyList<string> args)
    {
        if (args.Count > 1)
        {
            string arg = args[1];
            if (!float.TryParse(arg, out float value))
                goto syntax;

            ErmfishDefenseForce.instance.SetAggro(value, $"{CommandName} aggro");
        }
        return ErmfishDefenseForce.instance.CurrentAggro.ToString();

        syntax: return $"Syntax: {CommandName} aggro [value]";
    }

    public static string Reset()
    {
        ErmfishDefenseForce.instance.Reset();
        return null;
    }
}
