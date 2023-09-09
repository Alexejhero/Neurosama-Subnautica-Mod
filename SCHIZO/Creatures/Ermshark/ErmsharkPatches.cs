using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark;

[HarmonyPatch]
public static class ErmsharkPatches
{
    private static readonly FieldInfo damageTypeField = AccessTools.Field(typeof(Knife), nameof(Knife.damageType));
    private static readonly FieldInfo mainPlayerField = AccessTools.Field(typeof(Player), nameof(Player.main));
    private static readonly MethodInfo gameObjectGetter = AccessTools.DeclaredPropertyGetter(typeof(Component), nameof(Component.gameObject));
    private static readonly MethodInfo takeDamageMethod = AccessTools.Method(typeof(LiveMixin), nameof(LiveMixin.TakeDamage));

    [HarmonyPatch(typeof(Knife), nameof(Knife.OnToolUseAnim))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> SetPlayerDamageDealer(IEnumerable<CodeInstruction> instructions)
    {
        // replaces the null in this call with Player.main.gameObject
        //   liveMixin.TakeDamage(this.damage, position, this.damageType, null);
        List<CodeInstruction> codes = instructions.ToList();
        for (int i = 0; i < codes.Count; i++)
        {
            CodeInstruction curr = codes[i];

            if (curr.LoadsField(damageTypeField))
            {
                CodeInstruction loadDealerInst = codes[i + 1];
                CodeInstruction takeDamageCall = codes[i + 2];
                if (loadDealerInst.opcode == OpCodes.Ldnull && takeDamageCall.Calls(takeDamageMethod))
                {
                    //LOGGER.LogDebug($"Before patch:\n\t{string.Join("\n\t", codes.GetRange(i, 3))}");
                    loadDealerInst.opcode = OpCodes.Ldsfld;
                    loadDealerInst.operand = mainPlayerField;
                    CodeInstruction loadPlayerObject = new(OpCodes.Callvirt, gameObjectGetter);
                    codes.Insert(i+2, loadPlayerObject);
                    //LOGGER.LogDebug($"After patch:\n\t{string.Join("\n\t", codes.GetRange(i, 4))}");
                    break;
                }
            }
        }

        return codes;
    }
}
