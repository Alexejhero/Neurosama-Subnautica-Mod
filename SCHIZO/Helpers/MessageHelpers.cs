using System.Collections.Generic;

namespace SCHIZO.Helpers;

public static class MessageHelpers
{
    public static bool SuppressOutput = false;

    public static void WriteCommandOutput(string message, bool? suppressOutput = null)
    {
        ErrorMessage.AddMessage(GetCommandOutput(message, suppressOutput));
    }

    public static string GetCommandOutput(string message, bool? suppressOutput = null)
    {
        if (suppressOutput ?? SuppressOutput) return null;
        return message;
    }

    public static string TechTypeNotFound(string techTypeName)
    {
        IEnumerable<string> techTypeNamesSuggestion = TechTypeExtensions.GetTechTypeNamesSuggestion(techTypeName);
        return MessageHelpers.GetCommandOutput($"Could not find tech type for '{techTypeName}'. Did you mean:\n{string.Join("\n", techTypeNamesSuggestion)}");
    }
}
