using SCHIZO.Helpers;

namespace SCHIZO.Commands.Output;

#nullable enable
internal sealed class UiMessageSink : ISink
{
    /// <summary>
    /// Whether to suppress output if the command was invoked silently.
    /// </summary>
    public bool IsSneaky { get; private set; } = true;

    public static UiMessageSink Sneaky { get; } = new();
    public static UiMessageSink Loud { get; } = new() { IsSneaky = false };
    private UiMessageSink() { }

    public bool TryConsume(ref object? output)
    {
        bool suppress = IsSneaky && MessageHelpers.SuppressOutput;
        if (!suppress)
            ErrorMessage.AddMessage(output?.ToString());
        return false;
    }
}
