namespace SCHIZO.API.Helpers;

public class MessageHelpers
{
    public static bool SuppressOutput = false;

    public static void WriteCommandOutput(string message, bool? suppressOutput = null)
    {
        ErrorMessage.AddMessage(GetCommandOutput(message, suppressOutput));
    }

    public static string GetCommandOutput(string message, bool? suppressOutput = null)
    {
        if (suppressOutput ?? SuppressOutput) return null;
        return message;
    }
}
