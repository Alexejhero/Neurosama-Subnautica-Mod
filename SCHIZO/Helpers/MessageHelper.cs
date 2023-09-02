namespace SCHIZO.Helpers;
public class MessageHelper
{
    public static bool SuppressOutput = false;

    public static void WriteCommandOutput(string message, bool? suppressOutput = null)
    {
        if (suppressOutput ?? SuppressOutput) return;
        ErrorMessage.AddMessage(message);
    }
}
