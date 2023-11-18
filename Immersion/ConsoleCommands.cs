namespace Immersion;

internal class ConsoleCommands : MonoBehaviour
{
    public void Awake()
    {
        DevConsole.RegisterConsoleCommand(this, "immersion", false, false);
    }

    public static void OnConsoleCommand_immersion(NotificationCenter.Notification n)
    {
        List<string> args = n.data?.OfType<string>().ToList();
        bool success = ProcessCommand(args);
        if (success)
        {
            // something
        }
        else
        {
            // something else
        }
    }

    private static bool ProcessCommand(List<string> args)
    {
        if (args == null || args.Count < 1) return false;
        return args[0] switch
        {
            "set" => SetGlobal(args),
            "enable" or "disable" => EnableDisable(args),
            _ => false
        };
    }

    private static bool SetGlobal(List<string> args)
    {
        if (args.Count < 2 || args[0] != "set") return false;
        string fieldName = args[1];
        // preserve e.g. `set player Big Bob`
        string newValue = string.Join(" ", args.Skip(2));
        Globals.Strings[fieldName] = newValue;
        LOGGER.LogMessage($"{fieldName} set to {newValue}");
        return true;
    }

    private static bool EnableDisable(List<string> args)
    {
        if (args.Count < 1) return false;
        // `immersion disable` should disable the whole thing
        if (args.Count == 1) args.Add("all");

        bool enable = args[0] == "enable";
        foreach (string typeName in args.Skip(1))
        {
            if (typeName == "all")
            {
                foreach (Type type in Tracker.trackerTypes.Values)
                    SetComponentEnabled(type, enable);
            }
            else if (typeName == "sender") // for convenience
            {
                SetComponentEnabled(typeof(Sender), enable);
            }
            else
            {
                if (Tracker.trackerTypes.TryGetValue(typeName, out Type type))
                    SetComponentEnabled(type, enable);
            }
        }

        return true;
    }

    public static void SetComponentEnabled(Type type, bool enable)
    {
        Behaviour component = (Behaviour)PLUGIN_OBJECT.GetComponent(type);
        component.enabled = enable;
        if (component is Tracker tracker)
            tracker.startEnabled = enable;
    }
}
