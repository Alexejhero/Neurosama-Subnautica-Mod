using System.Collections.Generic;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Base;
using SCHIZO.Commands.Input;

namespace SCHIZO.SwarmControl.Redeems.Alerts;

#nullable enable
[CommandCategory("Alerts")]
[Redeem(Name = "redeem_attachalert",
    DisplayName = "Attach Alert",
    Description = "Attach an alert to an entity.",
    Export = false
)]
internal class AttachAlert() : ProxyCommand<MethodCommand>("contentalerts attach")
{
    public override IReadOnlyList<Parameter> Parameters { get; } = [
        new(new("techType", "Entity Type", "Type of entity to attach alerts to."), typeof(CommonTechTypes))
    ];

    protected override Dictionary<string, object?>? GetTargetArgs(Dictionary<string, object?>? proxyArgs)
    {
        if (proxyArgs is null)
            return null;
        NamedArgs args = new(proxyArgs);
        Dictionary<string, object?> targetArgs = [];

        if (args.TryGetValue("techType", out CommonTechTypes techType))
        {
            targetArgs["techType"] = techType.ToString();
        }

        return targetArgs;
    }
}
