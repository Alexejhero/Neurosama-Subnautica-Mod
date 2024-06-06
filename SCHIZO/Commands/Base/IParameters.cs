using System.Collections.Generic;

namespace SCHIZO.Commands.Base;

internal interface IParameters
{
    IReadOnlyList<Parameter> Parameters { get; }
}
