using System.Linq;
using Newtonsoft.Json;
using SCHIZO.Commands.Base;
using SwarmControl.Shared.Models.Game;

namespace SCHIZO.SwarmControl.Redeems;

internal sealed class Redeem(RedeemAttribute attr, Command command)
{
    public string Id { get; set; } = attr.Name;
    public string Title { get; set; } = attr.DisplayName;
    public string Description { get; set; } = attr.Description;
    public ParameterModel[] Args { get; set; } = command is not IParameters p ? []
        : p.Parameters.Select(p => new ParameterModel(p)).ToArray();
    public AnnounceType Announce { get; set; } = attr.Announce;
    public bool Moderated { get; set; } = attr.Moderated;

    public string Image { get; set; } = "UNSET";
    public int Price { get; set; } = 99999999;
    public string Sku { get; set; } = "UNSET";
    public bool Disabled { get; set; }
    public bool Hidden { get; set; }

    [JsonIgnore]
    public Command Command { get; } = command;
}
