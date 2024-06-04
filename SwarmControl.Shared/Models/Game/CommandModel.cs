using System.Collections.Generic;

namespace SwarmControl.Shared.Models.Game;
#nullable enable
public class CommandModel
{
    public required string Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public string? UsageRemarks { get; set; }
    public required IReadOnlyList<ParameterModel> Parameters { get; set; }
}
