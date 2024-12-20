using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Input;
using SwarmControl.Models.Game;

namespace SCHIZO.SwarmControl.Redeems.PayAttentionToMeStreamer;

#nullable enable
[Redeem(
    Name = "redeem_say",
    DisplayName = "Send Text",
    Description = "Display text in the top left of the screen",
    Export = false, /// deprecated in favor of <see cref="Hint"/>
    Announce = false // the message itself is the announcement
)]
internal class Say() : ProxyCommand<MethodCommand>("say")
{
    public override IReadOnlyList<Parameter> Parameters => [
        new TextParameter(new NamedModel("message", "Message", "The message to display")) {
            MinLength = 1
        }
    ];

    protected override JsonContext GetContextForTarget(JsonContext proxyCtx)
    {
        RemoteInput input = proxyCtx.JsonInput;
        input.Model.Announce = false;
        string submitter = input.Model.GetDisplayName();
        NamedArgs args = input.GetNamedArguments();
        args.TryGetValue("message", out string? message);
        if (string.IsNullOrWhiteSpace(message))
            message = "(no message)";
        args["message"] = $"{submitter}: {message}";
        return base.GetContextForTarget(proxyCtx);
    }

    protected override Dictionary<string, object?> GetTargetArgs(Dictionary<string, object?>? proxyArgs)
    {
        if (proxyArgs is null) return [];

        return new Dictionary<string, object?>
        {
            { "message", proxyArgs["message"] }
        };
    }
}
