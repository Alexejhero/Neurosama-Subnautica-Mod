using SCHIZO.Helpers;

namespace SCHIZO.Commands.Output;

internal sealed class UiMessageSink : ISink
{
    /// <summary>
    /// Whether to suppress output when the command was invoked from Twitch.
    /// </summary>
    public bool IsSneaky { get; private set; } = true;

    public static readonly UiMessageSink Sneaky = new();
    public static readonly UiMessageSink Loud = new() { IsSneaky = false };
    private UiMessageSink() { }

    public bool TryConsume(object output)
    {
        bool suppress = IsSneaky && MessageHelpers.SuppressOutput;
        if (!suppress)
            ErrorMessage.AddMessage(output.ToString());
        return false;
    }
}
