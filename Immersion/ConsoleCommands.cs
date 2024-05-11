using Immersion.Formatting;
using Immersion.Trackers;
using Nautilus.Commands;
using Nautilus.Handlers;

namespace Immersion;

internal class ConsoleCommands : MonoBehaviour
{
    public static readonly string Command = PLUGIN_NAME.ToLower();
    public static string SetUsage => "set <field> [value]";
    public static string SetExample => "set player Joe";
    public static string EnableDisableUsage => "<enable|disable> [tracker...]";
    public static string EnableDisableExample => $"disable {nameof(Empathy)} {nameof(Backseating)}";
    public static string ManualMuteUsage => "<mute|unmute>";
    public static string ManualSendUsage => "<react|send> <message>";
    public static string ManualSendExample => "react Behind you!";
    public static string ForcePrioUsage => "<prio|noprio>";

    public void Awake()
    {
        ConsoleCommandsHandler.RegisterConsoleCommands(typeof(ConsoleCommands));
    }

    [ConsoleCommand(PLUGIN_NAME)]
    public static string PluginControlCommand(params string[] args)
        => ProcessCommand(args);

    private static string ProcessCommand(IReadOnlyList<string> args)
    {
        if (args is not [_, ..]) // shorter to type than { Count: > 0 } ;)
            return $"""
                Usage:
                {Command} {SetUsage}
                    (e.g. `{Command} {SetExample}`)
                {Command} {EnableDisableUsage}
                    (e.g. `{Command} {EnableDisableExample}`)
                {Command} {ManualMuteUsage}
                {Command} {ManualSendUsage}
                    (e.g. `{Command} {ManualSendExample}`)
                {Command} {ForcePrioUsage}
                """;
        string subCommand = args[0].ToLower();
        return subCommand switch
        {
            "set" => SetGlobal(args),
            "enable" or "disable" => EnableDisable(args),
            "mute" or "unmute" => ManualMute(subCommand is "mute"),
            "react" or "send" => ManualSend(args),
            "prio" or "noprio" => SetForceLowPrio(subCommand is "noprio"),
            _ => $"Unknown sub-command {subCommand}, type `{Command}` to see usage"
        };
    }

    private static string SetGlobal(IReadOnlyList<string> args)
    {
        if (args is not ["set", string fieldName, ..])
            return $"Usage: `{Command} {SetUsage}`";

        // preserve e.g. `set player Big Bob`
        string newValue = string.Join(" ", args.Skip(2));

        switch(fieldName)
        {
            case "player":
                Globals.PlayerName = newValue;
                return $"Player name set to {newValue}";
            case "url":
                Globals.BaseUrl = newValue;
                return $"Base URL set to {newValue}";
            case "pronouns":
                return SetPronouns(newValue);
            default:
                return null;
        }
    }

    private static string EnableDisable(IReadOnlyList<string> args)
    {
        if (args.Count == 0) return $"Usage: {Command} {EnableDisableUsage}";
        bool enable = args[0] == "enable";
        // `disable` should disable the whole thing
        if (args.Count == 1)
        {
            // to be used in an emergency (e.g. if some tracker is causing issues live and we don't know which one)
            foreach (Type type in Tracker.trackerTypes.Values)
                SetComponentEnabled(type, enable);
            return null;
        }

        foreach (string typeName in args.Skip(1))
        {
            if (typeName == "sender") // for convenience
            {
                SetComponentEnabled(typeof(Sender), enable);
            }
            else
            {
                if (Tracker.trackerTypes.TryGetValue(typeName, out Type type))
                {
                    SetComponentEnabled(type, enable);
                    LOGGER.LogDebug($"{(enable ? "enable" : "disable")}d {typeName}");
                }
                else
                {
                    LOGGER.LogDebug($"{typeName} not found");
                }
            }
        }

        return null;
    }

    private static string SetPronouns(string fullArg)
    {
        if (!PronounSet.TryParse(fullArg, out PronounSet pronounSet))
            return """
                Could not parse pronouns.
                Pronoun sets consist of six parts:
                  - <color=yellow>Subject</color> (e.g. <color=green>I</color>)
                  - <color=yellow>Object</color> (e.g. <color=green>me</color>)
                  - <color=yellow>Possessive</color> (e.g. <color=green>his</color>)
                  - <color=yellow>Contraction of 'is'</color> (e.g. <color=green>they're</color>)
                  - <color=yellow>Contraction of 'has'</color> (e.g. <color=green>you've</color>)
                  - <color=yellow>Reflexive</color> (e.g. <color=green>itself</color>)
                Predefined sets are: <color=orange>I</color>, <color=orange>you</color>, <color=orange>he</color>, <color=orange>she</color>, <color=orange>they</color>, <color=orange>it</color>
                For others, at least <color=yellow>Subj/Obj/Poss</color> are required.
                Example: he/him/his
                """;
        Globals.PlayerPronouns = pronounSet;
        return $"Pronouns set to <color=green>{pronounSet}</color>";
    }

    public static void SetComponentEnabled(Type type, bool enable)
    {
        Behaviour component = (Behaviour)COMPONENT_HOLDER.GetComponent(type);
        component.enabled = enable;
        if (component is Tracker tracker)
            tracker.startEnabled = enable;
    }

    private static string ManualMute(bool mute)
    {
        PlayingVO playingVO = COMPONENT_HOLDER.GetComponent<PlayingVO>();
        playingVO.forceNext = true; // send even if the tracker is disabled
        playingVO.Notify(mute);

        return null;
    }

    private static string ManualSend(IReadOnlyList<string> args)
    {
        Sender sender = COMPONENT_HOLDER.GetComponent<Sender>();
        Tracker.Priority prio = args[0] is "react" ? Tracker.Priority.High : Tracker.Priority.Low;
        string message = args.Skip(1).Join(delimiter: " ");
        sender.forceNext = true;
        sender.Send(Tracker.PickEndpoint(prio), message);

        return null;
    }

    private static string SetForceLowPrio(bool value)
    {
        Tracker.ForceLowPriority = value;
        return null;
    }
}
