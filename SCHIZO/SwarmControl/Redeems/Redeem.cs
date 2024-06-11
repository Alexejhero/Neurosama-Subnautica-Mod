using System.Linq;
using Newtonsoft.Json;
using SCHIZO.Commands.Base;
using SwarmControl.Models.Game;

namespace SCHIZO.SwarmControl.Redeems;

internal sealed class Redeem(RedeemAttribute attr, Command command)
{
    public string Id { get; set; } = attr.Name;
    public string Title { get; set; } = attr.DisplayName;
    public string Description { get; set; } = attr.Description;
    public ParameterModel[] Args { get; set; } = command is not IParameters p ? []
        : p.Parameters.Select(p => new ParameterModel(p)).ToArray();
    public bool Announce { get; set; } = attr.Announce;

    public string Image { get; set; } = "https://vedalai.github.io/swarm-control/Erm.png";
    public int Price { get; set; } = 1;
    public string Sku { get; set; } = "bits1";
    public bool Disabled { get; set; }
    public bool Hidden { get; set; }

    [JsonIgnore]
    public Command Command { get; } = command;
    [JsonIgnore]
    public bool Export { get; set; } = attr.Export;
}
