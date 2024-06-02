using System;
using Nautilus.Handlers;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Output;
using BZJukebox = Jukebox;
using TrackId = Jukebox.UnlockableTrack;

namespace SCHIZO.Jukebox;

[CommandCategory("Jukebox")]
internal static class JukeboxConsoleCommands
{
    [Command(Name = "unlocktrack",
        DisplayName = "Unlock Track",
        Description = "",
        Remarks = "Track can be specified by enum name or value (e.g. `Track1` or just `1`)",
        RegisterConsoleCommand = true)]
    public static object UnlockTrack([TakeRemaining] string track = "")
    {
        if (string.IsNullOrEmpty(track))
            return CommonResults.ShowUsage();
        if (!Player.main) return null;

        // Nautilus only patches Enum.Parse...
        if (!Enum.TryParse(track, true, out TrackId trackId) && !EnumHandler.TryGetValue(track, out trackId))
            return $"No such track '{track}'";

        BZJukebox.Unlock(trackId, true);
        return CommonResults.OK();
    }
}
