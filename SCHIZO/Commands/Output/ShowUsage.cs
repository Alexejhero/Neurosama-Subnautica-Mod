using System.Text;
using SCHIZO.Commands.Base;

namespace SCHIZO.Commands.Output;
internal class ShowUsage(Command command) : ISink
{
    public bool TryConsume(ref object output)
    {
        if (output is not CommonResults.ShowUsageResult)
            return false;

        string line = GetUsageString(command);
        output = string.IsNullOrEmpty(line)
            ? $"Could not determine usage for {command.Name}"
            : $"Usage: {line}";
        return false;
    }

    public static string GetUsageString(Command command)
    {
        StringBuilder sb = new();
        if (command is IParameters { Parameters: { } parameters})
        {
            sb.Append(command.Name);
            foreach (Parameter param in parameters)
            {
                bool isOptional = param.IsOptional || param.HasDefaultValue || param.Type.IsArray;
                string paramNameSurrounded = isOptional ? $"[{param.Name}]" : $"<{param.Name}>";
                sb.Append($" {paramNameSurrounded}");
            }
        }
        else if (command is CompositeCommand comp)
        {
            sb.AppendLine();
            foreach (Command sub in comp.SubCommands.Values)
            {
                sb.AppendLine($"{command.Name} {GetUsageString(sub)}");
            }
        }
        return sb.ToString();
    }
}
