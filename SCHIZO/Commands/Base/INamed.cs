namespace SCHIZO.Commands.Base;

#nullable enable
public interface INamed
{
    string Name { get; }
    string? DisplayName { get; }
    string? Description { get; }
}
