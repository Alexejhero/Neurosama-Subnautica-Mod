using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.ConsoleCommands;
using SCHIZO.Commands.Context;
using SCHIZO.Commands.Output;

namespace SCHIZO.SwarmControl.Redeems.Annoying;

#nullable enable
[Redeem(
    Name = "redeem_floodbase",
    DisplayName = "Flood Base"
)]
internal class FloodBaseRedeem() : ProxyCommand<FloodBase>("damagebase")
{
    public override IReadOnlyList<Parameter> Parameters => [];
    protected override object? ExecuteCore(CommandExecutionContext ctx)
    {
        if (!Player.main) return CommonResults.Deny("Requires a loaded game");

        return base.ExecuteCore(ctx);
    }

    protected override Dictionary<string, object?>? GetTargetArgs(Dictionary<string, object?>? proxyArgs)
    {
        return new()
        {
            { "damage", 1f }
        };
    }
}
