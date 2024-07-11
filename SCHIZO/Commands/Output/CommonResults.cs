using System;

namespace SCHIZO.Commands.Output;

public static class CommonResults
{
    // sizeof says 1 byte for the empty types (i'm guessing type marker or pointer padding)
    // aaaa i'm having withdrawals i miss rust enums

    /// <summary>Indicates that the command was executed successfully.</summary>
    public readonly record struct OKResult;

    /// <summary>
    /// Mostly for internal use; shows usage information to the user.<br/>
    /// This must <b>never</b> be returned from a redeem.
    /// </summary>
    public readonly record struct ShowUsageResult;

    /// <summary>
    /// An internal error occurred while trying to execute the command.
    /// </summary>
    public readonly record struct ExceptionResult(Exception Exception);

    /// <summary>
    /// Something went wrong and the command could not be executed cleanly.
    /// </summary>
    /// <param name="message">Message to return to the user. Will be logged on the backend, as well as posting a notification on Discord.</param>
    public readonly record struct ErrorResult(string message);

    // no MessageResult(string) because you can just return a string (may still add one in the future for log levels and such)

    /// <summary>
    /// Acts like <see cref="ErrorResult"/> in that the user is shown an error and the redeem gets refunded;<br/>
    /// The difference is that this does not trigger Discord logs.
    /// </summary>
    /// <param name="reason">Message to display to the user. This will be logged to the database but will not notify on Discord.</param>
    public readonly record struct DenyResult(string reason);

    public static OKResult OK() => new();
    /// <summary>
    /// The parameters supplied to the command were incorrect.<br/>
    /// Do <b>not</b> return this from a redeem.
    /// </summary>
    public static ShowUsageResult ShowUsage() => new();
    /// <summary>
    /// Commands automatically catch exceptions but you can still use this.
    /// </summary>
    public static ExceptionResult Exception(Exception e) => new(e);
    /// <summary>
    /// The command failed to execute due to an error.
    /// </summary>
    /// <param name="message">The message to return to the user.</param>
    public static ErrorResult Error(string message) => new(message);
    /// <summary>
    /// The command could not be executed due to a precondition (such as a cooldown).
    /// </summary>
    /// <param name="reason">Message to return to the user.</param>
    public static DenyResult Deny(string reason) => new(reason);
}
