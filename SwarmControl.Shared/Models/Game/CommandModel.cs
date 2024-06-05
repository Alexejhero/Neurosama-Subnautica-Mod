using System.Collections.Generic;

namespace SwarmControl.Shared.Models.Game;
#nullable enable
public class CommandModel : NamedModel
{
    public string? UsageRemarks { get; set; }
    public required IReadOnlyList<ParameterModel> Parameters { get; set; }
}
