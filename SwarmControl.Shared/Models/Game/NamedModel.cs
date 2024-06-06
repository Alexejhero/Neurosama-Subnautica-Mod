namespace SwarmControl.Shared.Models.Game;

#nullable enable
public class NamedModel(string name, string? displayName = null, string? description = null)
{
    public string Name { get; set; } = name;
    public string? DisplayName { get; set; } = displayName;
    public string? Description { get; set; } = description;

    protected internal NamedModel()
        : this(null!)
    { }
    public NamedModel(NamedModel other)
        : this(other.Name, other.DisplayName, other.Description)
    { }
    public static implicit operator NamedModel(string name)
        => new(name);
}
