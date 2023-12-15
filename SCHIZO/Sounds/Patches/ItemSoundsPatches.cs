using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SCHIZO.Helpers;
using SCHIZO.Sounds.Players;
using UnityEngine;

namespace SCHIZO.Sounds.Patches;

[HarmonyPatch]
public static class ItemSoundsPatches
{
    [HarmonyPatch(typeof(Survival), nameof(Survival.Eat))]
    public static class PlayCustomEatSound
    {
        [HarmonyTranspiler, UsedImplicitly]
        public static IEnumerable<CodeInstruction> Injector(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher matcher = new(instructions);
            matcher.Start();
            matcher.MatchForward(false,
                new CodeMatch(IS_BELOWZERO ? OpCodes.Ldloc_S : OpCodes.Ldloc_2),
                new CodeMatch(OpCodes.Ldc_I4, (int)TechType.Bladderfish),
                new CodeMatch(ci => ci.opcode == OpCodes.Bne_Un || ci.opcode == OpCodes.Bne_Un_S) // jit moment
            );
            if (matcher.IsValid)
            {
                matcher.Insert
                (
                    new CodeInstruction(OpCodes.Ldarg_1)
                        .MoveLabelsFrom(matcher.Instruction),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlayCustomEatSound), nameof(Patch)))
                );
            }
            else
            {
                LOGGER.LogError("Could not patch Survival.Eat - ItemSounds.eat will not play");
            }
            return matcher.InstructionEnumeration();
        }

        private static void Patch(GameObject useObj)
        {
            ItemSounds sounds = useObj.GetComponent<ItemSounds>();
            if (sounds) sounds.OnEat();
        }
    }

    // TODO get the first inventory item with the ingredients' techtype
    // the techtype passed to the function is actually the craft result
    // how did this ever work in the first place holy
    [HarmonyPatch(typeof(Crafter), nameof(Crafter.OnCraftingBegin))]
    [HarmonyPostfix]
    public static void PlayCustomCookSound(TechType techType)
    {
        const float delayPerItem = 0.1f;
#if BELOWZERO
        IEnumerable<NIngredient> ingredients = TechData.GetIngredients(techType);
#else
        NTechData techData = CraftData.techData[techType];
        IEnumerable<NIngredient> ingredients = techData._ingredients;
#endif
        foreach (NIngredient ingredient in ingredients)
        {
            for (int i = 0; i < ingredient.amount; i++)
                ItemSounds.OnCook(ingredient.techType, delayPerItem * i);
        }
    }
}
