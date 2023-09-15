using System.Collections.Generic;
using JetBrains.Annotations;
using Nautilus.Commands;
using SCHIZO.Attributes;
using SCHIZO.Extensions;
using SCHIZO.Helpers;
using UnityEngine;
using Random = System.Random;

namespace SCHIZO;

[LoadConsoleCommands]
public static class ConsoleCommands
{
    [ConsoleCommand("isekai"), UsedImplicitly]
    public static void OnConsoleCommand_isekai(string techTypeName, float percentage, float radius = 100)
    {
        if (!UWE.Utils.TryParseEnum(techTypeName, out TechType techType))
        {
            IEnumerable<string> techTypeNamesSuggestion = TechTypeExtensions.GetTechTypeNamesSuggestion(techTypeName);
            MessageHelpers.WriteCommandOutput($"Could not find tech type for '{techTypeName}'. Did you mean:\n{string.Join("\n", techTypeNamesSuggestion)}");
            return;
        }

        Random rand = new();
        foreach (TechTag tag in PhysicsHelpers.ObjectsInRange(Player.main.transform, radius).OfTechType(techType).SelectComponentInParent<TechTag>())
        {
            if (rand.NextDouble() <= percentage) Object.Destroy(tag.gameObject);
        }
    }
}
