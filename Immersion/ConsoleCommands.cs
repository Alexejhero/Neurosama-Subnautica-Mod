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
    public static string EnableDisableExample => $"disable {nameof(PlayingVO)} {nameof(StoryGoals)}";
    public static string ManualMuteUsage => "<mute|unmute>";
    public static string ManualSendUsage => "<react|send> <message>";
    public static string ManualSendExample => "react Behind you!";

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
                """;
        string subCommand = args[0].ToLower();
        return subCommand switch
        {
            "set" => SetGlobal(args),
            "enable" or "disable" => EnableDisable(args),
            "mute" or "unmute" => ManualMute(subCommand is "mute"),
            "react" or "send" => ManualSend(args),
            _ => $"Unknown sub-command {subCommand}, type `{Command}` to see usage"
        };
    }

    private static string SetGlobal(IReadOnlyList<string> args)
    {
        if (args is not ["set", string fieldName, ..])
            return $"Usage: `{Command} {SetUsage}`";

        // preserve e.g. `set player Big Bob`
        string newValue = string.Join(" ", args.Skip(2));
        if (fieldName == "pronouns") // hack but i don't care
            return SetPronouns(newValue);
        
        Globals.Strings[fieldName] = newValue;
        return $"{fieldName} set to {newValue}";
    }

    private static string EnableDisable(IReadOnlyList<string> args)
    {
        if (args.Count == 0) return $"Usage: {Command} {EnableDisableUsage}";
        bool enable = args[0] == "enable";
        // `disable` should disable the whole thing
        if (args.Count == 1)
        {
            // TODO maybe just disable the sender?
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
            return "Could not parse pronouns (example: \"he/him/his\")";
        Globals.PlayerPronouns = pronounSet;
        return null;
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
        // since it's manual we want to send even if PlayingVO is disabled
        PlayingVO playingVO = COMPONENT_HOLDER.GetComponent<PlayingVO>();
        playingVO.forceNext = true;
        playingVO.Send(mute);

        return null;
    }

    private static string ManualSend(IReadOnlyList<string> args)
    {
        Sender sender = COMPONENT_HOLDER.GetComponent<Sender>();
        Tracker.Priority prio = args[0] is "react" ? Tracker.Priority.High : Tracker.Priority.Low;
        string message = args.Skip(1).Join(delimiter: " ");
        sender.forceNext = true;
        _ = sender.Send(Tracker.PickEndpoint(prio), message);

        return null;
    }
}
