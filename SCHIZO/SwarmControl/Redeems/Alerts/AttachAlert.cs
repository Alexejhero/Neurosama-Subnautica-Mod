using System.Collections.Generic;
using SCHIZO.Commands.Attributes;
using SCHIZO.Commands.Base;

namespace SCHIZO.SwarmControl.Redeems.Alerts;

#nullable enable
[CommandCategory("Alerts")]
[Redeem(Name = "redeem_attachalert",
    DisplayName = "Attach Alert",
    Description = "Attach an alert to an entity.",
    Announce = false
)]
internal class AttachAlert() : ProxyCommand<MethodCommand>("contentalerts attach")
{
    public override IReadOnlyList<Parameter> Parameters { get; } = [
        new(new("techType", "Entity Type", "Type of entity to attach alerts to."), typeof(TechType))
    ];

    protected override Dictionary<string, object?>? GetTargetArgs(Dictionary<string, object?>? proxyArgs)
    {
        return [];
    }
}
