using System.Collections.Generic;
using SCHIZO.Commands.Base;

namespace SCHIZO.SwarmControl.Redeems;

internal interface IParameters
{
    IReadOnlyList<Parameter> Parameters { get; }
}
