using System.Collections;
using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Input;
using SCHIZO.Commands.Output;
using SCHIZO.Twitch;
using SwarmControl.Models.Game.Messages;
using UnityEngine;
using UWE;

namespace SCHIZO.SwarmControl.Redeems.PayAttentionToMeStreamer;

#nullable enable
[Redeem(
    Name = "redeem_addsignal",
    DisplayName = "Add Signal",
    Description = "Adds a signal marker to the world. After redeeming, your next Twitch chat message will name the beacon."
)]
internal class AddSignalRedeem : Command, IParameters
{
    public IReadOnlyList<Parameter> Parameters { get; } = [
        new Parameter(new("coords", "Coordinates", "Beacon coordinates"), typeof(Vector3), false)
    ];

    protected override object? ExecuteCore(CommandExecutionContext ctx)
    {
        RedeemMessage? model = (ctx.Input as RemoteInput)?.Model;
        TwitchUser? user = model?.User;
        if (user is null)
            return CommonResults.Error("Could not get user");
        if (!ctx.Input.GetNamedArguments().TryGetValue("coords", out Vector3 coords))
            return CommonResults.Error("Invalid coordinates");
        CoroutineHost.StartCoroutine(Coro(user, coords));
        return "Success - the next message you send in Twitch chat will be used to name the beacon.";
    }

    private IEnumerator Coro(TwitchUser user, Vector3 coords)
    {
        string? message = null;
        TwitchIntegration.AddNextMessageCallback(user, (msg) => message = msg ?? "(unnamed)");
        yield return new WaitUntil(() => message is { });
        CustomSignalManager.AddSignal(coords.x, coords.y, coords.z,
            $"[{user.DisplayName}]\n{message}");
    }
}
