using System.Collections.Generic;

namespace SwarmControl.Shared.Models.Game;
public class EnumDefinitionModel
{
    public string Name { get; set; }
    public required Dictionary<string, int> Values { get; set; }
}
