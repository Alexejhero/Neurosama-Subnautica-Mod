namespace SwarmControl.Shared.Models.Game;

public class ParameterModel : NamedModel
{
    public string Type { get; set; }
    public bool IsOptional { get; set; }
    public object DefaultValue { get; set; }
}
