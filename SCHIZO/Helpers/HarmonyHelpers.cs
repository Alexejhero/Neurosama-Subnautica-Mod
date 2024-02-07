using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine.UIElements.StyleSheets.Syntax;

namespace SCHIZO.Helpers;
internal static class HarmonyHelpers
{
    private static readonly OpCode[] _ldloc0to3Codes = [
        OpCodes.Ldloc_0,
        OpCodes.Ldloc_1,
        OpCodes.Ldloc_2,
        OpCodes.Ldloc_3
    ];
    private static readonly OpCode[] _stloc0to3Codes = [
        OpCodes.Stloc_0,
        OpCodes.Stloc_1,
        OpCodes.Stloc_2,
        OpCodes.Stloc_3,
    ];

    /// <summary>
    /// Checks whether this is an instruction that loads a local variable at the specified index (and of the specified type).
    /// </summary>
    /// <param name="instr">Instruction to check.</param>
    /// <param name="index">If specified, checks whether the local variable has this index.</param>
    /// <param name="type">
    /// If specified, checks that the type of the local variable matches.<br/>
    /// If <paramref name="index"/> is between 0 and 3 (inclusive), <paramref name="method"/> must also be specified.
    /// </param>
    /// <param name="method">Required when checking <paramref name="type"/> for <paramref name="index"/> between 0 and 3 (inclusive).</param>
    /// <remarks>
    /// IMPORTANT: <paramref name="method"/> <b>must</b> be specified if checking the <paramref name="type"/> for <paramref name="index"/> between 0 and 3 (inclusive), since they use special instructions (e.g. <see cref="OpCodes.Ldloc_0"/>) that don't have a <see cref="LocalBuilder"/> operand.<br/>
    /// In all other cases (<paramref name="index"/> over 3, not checking <paramref name="type"/>), <paramref name="method"/> can be omitted or set to <see langword="null"/>.
    /// </remarks>
    /// <returns>A <see cref="bool"/> that indicates whether the given instruction matches the specified criteria.</returns>
    public static bool LoadsLocal(this CodeInstruction instr, int? index = null, Type type = null, MethodBody method = null)
    {
        return AffectsLocal(instr, false, index, type, method);
    }

    /// <summary>
    /// Check whether this is an instruction that stores a local at the specified index (and of the specified type).
    /// </summary>
    /// <param name="instr">Instruction to check.</param>
    /// <param name="index">If specified, checks whether the local is at this index in the method.</param>
    /// <param name="type">
    /// If specified, checks that the type of the local matches.<br/>
    /// If <paramref name="index"/> is under 4, <paramref name="method"/> must also be specified.
    /// </param>
    /// <param name="method">Required when checking <paramref name="type"/> for <paramref name="index"/> under 4.</param>
    /// <remarks>
    /// IMPORTANT: <paramref name="method"/> <b>must</b> be specified if checking the <paramref name="type"/> for <paramref name="index"/> under 4, since they use special instructions (e.g. <see cref="OpCodes.Stloc_0"/>) that don't have a <see cref="LocalBuilder"/> operand.<br/>
    /// In all other cases (<paramref name="index"/> over 3, not checking <paramref name="type"/>), <paramref name="method"/> can be omitted or set to <see langword="null"/>.
    /// </remarks>
    /// <returns>A <see cref="bool"/> that indicates whether the given instruction matches the specified criteria.</returns>
    public static bool StoresLocal(this CodeInstruction instr, int? index = null, Type type = null, MethodBody method = null)
    {
        return AffectsLocal(instr, true, index, type, method);
    }

    private static bool AffectsLocal(this CodeInstruction instr, bool stores, int? index = null, Type type = null, MethodBody method = null)
    {
        if (index is < 0) throw new ArgumentOutOfRangeException(nameof(index), "index must be at least 0");

        if (stores ? !instr.IsStloc() : !instr.IsLdloc())
            return false;

        OpCode[] codes0to3 = stores
            ? _stloc0to3Codes
            : _ldloc0to3Codes;

        int localIndex;
        Type localType;

        int index0to3 = Array.IndexOf(codes0to3, instr.opcode);
        if (index0to3 > 0)
        {
            localIndex = index0to3;
            localType = type is { }
                ? method?.LocalVariables[index.Value].LocalType
                    ?? throw new ArgumentNullException(nameof(method), $"local variable is at index {index0to3}, cannot check type without method")
                : null;
        }
        else
        {
            LocalBuilder local = (LocalBuilder)instr.operand;
            localIndex = local.LocalIndex;
            localType = local.LocalType;
        }

        return (index == null || index == localIndex)
            && (type == null || type == localType);
    }

    /// <summary>
    /// Get an instruction that loads the specified local.<br/>
    /// This is a convenience function that automatically handles special opcodes for locals at index 0 to 3 (e.g. <see cref="OpCodes.Ldloc_0"/>).
    /// </summary>
    /// <param name="local">Local to load.</param>
    /// <param name="indirect">Whether to load the value or the address of the local. This matters for struct types.</param>
    /// <returns>A <see cref="CodeInstruction"/> with the correct <see cref="OpCode"/> and operand for the operation.</returns>
    public static CodeInstruction LoadInstruction(this LocalVariableInfo local, bool indirect = false)
    {
        return LocalInstruction(local, indirect, false);
    }
    /// <summary>
    /// Get an instruction that stores the specified local.<br/>
    /// This is a convenience function that automatically handles special opcodes for locals at index 0 to 3 (e.g. <see cref="OpCodes.Stloc_0"/>).
    /// </summary>
    /// <param name="local">Local to store.</param>
    /// <returns>A <see cref="CodeInstruction"/> with the correct <see cref="OpCode"/> and operand for the operation.</returns>
    public static CodeInstruction StoreInstruction(this LocalVariableInfo local)
    {
        return LocalInstruction(local, false, true);
    }

    private static CodeInstruction LocalInstruction(this LocalVariableInfo local, bool indirect = false, bool store = false)
    {
        OpCode opcode;
        bool needsOperand;

        if (store && indirect)
            throw new InvalidOperationException("both store and indirect were specified; no instructions exist for this combination");

        if (local.LocalIndex < 4 && !indirect)
        {
            opcode = (store ? _stloc0to3Codes : _ldloc0to3Codes)[local.LocalIndex];
            needsOperand = false;
        }
        else
        {
            needsOperand = true;
            bool shortForm = local.LocalIndex <= byte.MaxValue;
            opcode = (indirect, shortForm) switch
            {
                (false, false) => store ? OpCodes.Stloc : OpCodes.Ldloc,
                (false, true) => store ? OpCodes.Stloc_S : OpCodes.Ldloc_S,
                // store can't be indirect
                (true, false) => OpCodes.Ldloca,
                (true, true) => OpCodes.Ldloca_S,
            };
        }
        return new CodeInstruction(opcode, needsOperand ? local : null);
    }

    public static CodeInstruction CallInstruction(this MethodInfo method)
    {
        return method.IsVirtual
            ? new CodeInstruction(OpCodes.Callvirt, method)
            : new CodeInstruction(OpCodes.Call, method);
    }
}
