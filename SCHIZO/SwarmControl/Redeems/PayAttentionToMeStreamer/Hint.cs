using System.Collections;
using System.Collections.Generic;
using SCHIZO.Commands;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Input;
using SCHIZO.Commands.Output;
using SCHIZO.SwarmControl.Models.Game.Messages;
using UnityEngine;
using UWE;

namespace SCHIZO.SwarmControl.Redeems.PayAttentionToMeStreamer;

#nullable enable
[Redeem(
    Name = "redeem_hint",
    DisplayName = "Send Message",
    Description = "Your next Twitch chat message will be displayed in-game",
    Announce = false // the message itself is the announcement
)]
internal class Hint : Command, IParameters
{
    public IReadOnlyList<Parameter> Parameters => [];

    protected override object? ExecuteCore(CommandExecutionContext ctx)
    {
        RedeemMessage? model = (ctx.Input as RemoteInput)?.Model;
        TwitchUser? user = model?.User;
        if (user is null)
            return CommonResults.Error("Could not get user");
        CoroutineHost.StartCoroutine(Coro(user));
        return "Success - the next message you send in Twitch chat will be displayed in-game.";
    }

    private IEnumerator Coro(TwitchUser user)
    {
        string? message = null;
        Twitch.TwitchIntegration.AddNextMessageCallback(user, (msg) => message = msg ?? "(no message)");
        yield return new WaitUntil(() => message is { });
        DevCommands.Hint($"{user.DisplayName}: {message}");
    }
}
