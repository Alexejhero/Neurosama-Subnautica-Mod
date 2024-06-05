using System;
using System.Collections.Generic;
using SwarmControl.Shared.Models.Game.Messages;

namespace SCHIZO.Commands.Input;

public sealed record JsonInputModel
{
    /// <summary>ID for logging/auditing.</summary>
    public Guid CorrelationId { get; set; }
    /// <summary>
    /// Which kind of user invoked the command.
    /// </summary>
    public CommandInvocationSource Source { get; set; }
    /// <summary>
    /// Twitch user that invoked the command.<br/>
    /// Crowd Control users can choose to redeem anonymously.
    /// </summary>
    public TwitchUser User { get; set; }
    /// <summary>
    /// Timestamp for queueing/logging/auditing.
    /// </summary>
    public ulong Timestamp { get; set; }
    /// <summary>
    /// Whether to announce the invocation in-game.<br/>
    /// Does not affect logging.
    /// </summary>
    public bool Announce { get; set; }

    /// <summary>Command name.</summary>
    /// <remarks>
    /// In the case of composite commands, this is the whole string encompassing the parent command(s) and the subcommand name, e.g. <c>"command subcommand subsubcommand"</c>.<br/>
    /// For subcommands, this will start with the current command's name (e.g. <c>"subcommand subsubcommand"</c>).
    /// </remarks>
    public string Command { get; set; }

    /// <summary>
    /// Named arguments for the invocation.
    /// </summary>
    public Dictionary<string, object> Args { get; set; }
}
