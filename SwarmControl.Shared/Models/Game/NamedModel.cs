namespace SwarmControl.Shared.Models.Game;

#nullable enable
public abstract class NamedModel
{
    public required string Name { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
}
