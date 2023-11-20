using Immersion.Formatting;
using Immersion.Trackers;

namespace Immersion;

internal class ConsoleCommands : MonoBehaviour
{
    public static readonly string Command = PLUGIN_NAME.ToLower();
    public static string SetUsage => "set <field> [value]";
    public static string SetExample => "set player Joe";
    public static string EnableDisableUsage => "<enable|disable> [tracker]";
    public static string EnableDisableExample => $"disable {nameof(Trackers.PlayingVO)}";

    public void Awake()
    {
        DevConsole.RegisterConsoleCommand(this, Command, false, false);
    }

    public static void OnConsoleCommand_immersion(NotificationCenter.Notification n)
    {
        List<string> args = n.data?.OfType<string>().ToList();
        string msg = ProcessCommand(args);
        if (msg == null) return;

        ErrorMessage.AddMessage($"[{PLUGIN_NAME}] {msg}");
    }

    private static string ProcessCommand(List<string> args)
    {
        if (args is not [..])
            return $"""
                Usage:
                {Command} {SetUsage}
                    (e.g. `{Command} {SetExample}`)
                {Command} {EnableDisableUsage}
                    (e.g. `{Command} {EnableDisableExample}`)
                """;
        return args[0] switch
        {
            "set" => SetGlobal(args),
            "enable" or "disable" => EnableDisable(args),
            _ => $"Unknown sub-command {args[0]}, type `{Command}` to see usage"
        };
    }

    private static string SetGlobal(List<string> args)
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

    private static string EnableDisable(List<string> args)
    {
        if (args.Count < 1) return $"Usage: {Command} {EnableDisableUsage}";
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
                    SetComponentEnabled(type, enable);
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
        Behaviour component = (Behaviour)PLUGIN_OBJECT.GetComponent(type);
        component.enabled = enable;
        if (component is Tracker tracker)
            tracker.startEnabled = enable;
    }
}
