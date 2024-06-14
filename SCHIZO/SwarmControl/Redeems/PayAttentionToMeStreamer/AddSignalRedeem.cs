using System.Collections.Generic;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Input;
using UnityEngine;

namespace SCHIZO.SwarmControl.Redeems.PayAttentionToMeStreamer;

#nullable enable
[Redeem(
    Name = "redeem_addsignal",
    DisplayName = "Add Beacon",
    Description = "Adds a beacon ping to the map."
)]
internal class AddSignalRedeem() : ProxyCommand<MethodCommand>("addsignal")
{
    public override IReadOnlyList<Parameter> Parameters { get; } = [
        new Parameter(new("coords", "Coordinates", "Beacon coordinates"), typeof(Vector3), false),
        new Parameter(new("name", "Name", "Beacon name"), typeof(string), false)
    ];

    protected override Dictionary<string, object?>? GetTargetArgs(Dictionary<string, object?>? proxyArgs)
    {
        if (proxyArgs is null) return [];

        NamedArgs args = new(proxyArgs);
        Dictionary<string, object?> targetArgs = [];
        if (!args.TryGetValue("coords", out Vector3 coords) || !args.TryGetValue("name", out string? name))
            return targetArgs;

        return new()
        {
            { "x", coords.x },
            { "y", coords.y },
            { "z", coords.z },
            { "signalName", name }
        };
    }
}
