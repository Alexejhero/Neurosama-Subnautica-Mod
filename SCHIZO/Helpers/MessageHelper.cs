namespace SCHIZO.Helpers;
public class MessageHelper
{
    public static bool SuppressOutput = false;
    public static void WriteCommandOutput(string message, bool? suppressOutput = null)
    {
        bool doSuppress = SuppressOutput;
        if (suppressOutput is { } overrideSuppressOutput)
            doSuppress = overrideSuppressOutput;

        if (doSuppress) return;

        ErrorMessage.AddMessage(message);
    }
}
