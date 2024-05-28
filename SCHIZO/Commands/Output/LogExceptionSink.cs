using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCHIZO.Commands.Output;
internal sealed class LogExceptionSink : ISink
{
    public object ReplaceWith { get; set; } = "Someone tell Alex there is a problem";
    public bool TryConsume(ref object output)
    {
        Exception ex = (output as Exception)
            ?? (output as CommonResults.ExceptionResult?)?.Exception;
        //Exception ex = output switch
        //{
        //    Exception directEx => directEx,
        //    CommonResults.ExceptionResult { Exception: Exception contained } => contained,
        //    _ => null
        //};

        if (ex is null) return false;

        LOGGER.LogError(ex);
        if (ReplaceWith is null)
            return true;
        output = ReplaceWith;
        return false;
    }
}
