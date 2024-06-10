namespace SwarmControl.Shared.Models.Game;

#nullable enable
public class NamedModel(string name, string? title = null, string? description = null)
{
    public string Name { get; set; } = name;
    public string? Title { get; set; } = title;
    public string? Description { get; set; } = description;

    protected internal NamedModel()
        : this(null!)
    { }
    public NamedModel(NamedModel other)
        : this(other.Name, other.Title, other.Description)
    { }
    public static implicit operator NamedModel(string name)
        => new(name);
}
