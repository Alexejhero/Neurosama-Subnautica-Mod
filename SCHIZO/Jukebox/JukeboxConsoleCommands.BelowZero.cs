using System;
using JetBrains.Annotations;
using Nautilus.Commands;
using Nautilus.Handlers;
using SCHIZO.ConsoleCommands;
using TrackId = Jukebox.UnlockableTrack;

namespace SCHIZO.Jukebox;
[RegisterConsoleCommands]
internal static class JukeboxConsoleCommands
{
    [ConsoleCommand("unlocktrack"), UsedImplicitly]
    public static string OnConsoleCommand_unlocktrack(params string[] trackArgs)
    {
        string track = string.Join(" ", trackArgs);
        if (string.IsNullOrEmpty(track))
            return "Usage: unlocktrack <track id>\nTrack ID can be specified by enum name or value (e.g. `Track1` or just `1`)";
        // Nautilus only patches Enum.Parse...
        if (!Enum.TryParse(track, true, out TrackId trackId) && !EnumHandler.TryGetValue(track, out trackId))
            return $"No such track '{track}'";

        if (!Player.main) return "Jukebox tracks can only be unlocked in-game";
        global::Jukebox.Unlock(trackId, true);
        return null;
    }
}
