using System.Collections.Generic;
using JetBrains.Annotations;

namespace SCHIZO.Items.Gymbag
{
    [UsedImplicitly]
    public sealed partial class GymbagLoader : ItemLoader
    {
        protected override HashSet<string> AllowedClassIds => ["gymbag", "quantumgymbag"];
    }
}
