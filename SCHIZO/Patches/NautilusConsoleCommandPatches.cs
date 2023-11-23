using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Nautilus.Commands;
using Nautilus.Patchers;
using Nautilus.Utility;

namespace SCHIZO.Patches;

[HarmonyPatch]
public static class NautilusConsoleCommandPatches
{
    // TODO do this whole mess properly as a PR to nautilus
    // right now we don't have time to wait for the next release

    [HarmonyPatch(typeof(Parameter), MethodType.Constructor, typeof(ParameterInfo))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> AllowNullableValueTypesAndArrays(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher matcher = new(instructions);
        matcher.SearchForward(ci => ci.opcode == OpCodes.Call
            && matcher.Operand is MethodInfo { Name: "Contains" } mi
            && mi.DeclaringType == typeof(Enumerable));
        if (matcher.IsValid)
        {
            // pop off the 'this IEnumerable<Type>' and 'Type' args that were previously on the stack
            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Pop), new CodeInstruction(OpCodes.Pop));

            // replace method call with our function
            matcher.InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_1));
            matcher.Operand = new Func<ParameterInfo, bool>(IsValid).Method;
        }

        return matcher.InstructionEnumeration();
    }

    private static bool IsValid(ParameterInfo paramInfo)
    {
        Type type = paramInfo.ParameterType;
        if (type.IsArray)
        {
            // arrays MUST be a "params T[]" parameter
            // this enforces them being last *and* only having one
            if (!paramInfo.IsDefined(typeof(ParamArrayAttribute), false))
                return false;
            type = type.GetElementType();
        }
        if (Nullable.GetUnderlyingType(type) is { } actualValueType)
            type = actualValueType;
        bool isSupported = Parameter.TypeConverters.ContainsKey(type);
        return isSupported;
    }

    [HarmonyPatch(typeof(Parameter), nameof(Parameter.Parse))]
    [HarmonyPrefix]
    public static bool ParseReplacement(Parameter __instance, string input, out object __result)
    {
        __result = ParseInner(input, __instance.ParameterType);
        return false;

        static object ParseInner(string input, Type paramType)
        {
            if (paramType.IsArray)
            {
                paramType = paramType.GetElementType();
            }

            if (Nullable.GetUnderlyingType(paramType) is { } actualType)
            {
                paramType = actualType;
                if (string.IsNullOrEmpty(input) || string.Equals(input, "null", StringComparison.OrdinalIgnoreCase))
                    return null;
            }
            return Parameter.TypeConverters[paramType](input);
        }
    }

    [HarmonyPatch(typeof(ConsoleCommandsPatcher), nameof(ConsoleCommandsPatcher.HandleCommand))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> CheckForDifferentMissingValue(IEnumerable<CodeInstruction> instructions)
    {
        // replace a `X == null` check with a different value (`X == Type.Missing`)
        // since we accept nullables now, null is a valid value
        CodeMatcher matcher = new(instructions);
        matcher.MatchForward(true,
            // CodeMatch matches using a List.Contains... which uses object.Equals
            //new CodeMatch(OpCodes.Ldloc_S, 4),
            new CodeMatch(ci => ci.opcode == OpCodes.Ldloc_S && ci.operand is LocalBuilder { LocalIndex: 4 }),
            new CodeMatch(ci => ci.opcode == OpCodes.Ldloc_S && ci.operand is LocalBuilder { LocalIndex: 8 }),
            new CodeMatch(OpCodes.Ldelem_Ref),
            // disasm shows brtrue.s but debugger shows brtrue... screw it, handle both
            new CodeMatch(ci => ci.opcode == OpCodes.Brtrue_S || ci.opcode == OpCodes.Brtrue));
        if (matcher.IsValid)
        {
            matcher.InsertAndAdvance(
                new CodeInstruction(OpCodes.Call, new Func<object, bool>(IsParameterParsedOK).Method)
            );
            matcher.Opcode = matcher.Opcode == OpCodes.Brtrue_S
                ? OpCodes.Brfalse_S
                : OpCodes.Brfalse;
        }

        return matcher.InstructionEnumeration();
    }

    [HarmonyPatch(typeof(ConsoleCommand), MethodType.Constructor, typeof(string), typeof(MethodInfo), typeof(bool), typeof(object))]
    [HarmonyPostfix]
    public static void Megacringe(ConsoleCommand __instance)
    {
        __instance.SetInstanceField("<Parameters>k__BackingField",
            __instance.Parameters.ToList());
    }

    [HarmonyPatch(typeof(ConsoleCommand), nameof(ConsoleCommand.TryParseParameters))]
    [HarmonyPrefix]
    public static bool TryParseParametersReplacement(ConsoleCommand __instance, IEnumerable<string> inputParameters, out object[] parsedParameters, out bool __result)
    {
        List<string> input = inputParameters.ToList();
        List<Parameter> parameters = (List<Parameter>)__instance.Parameters;
        int consumed = BetterTryParseParameters(input, parameters, out parsedParameters);
        // TODO: transpile/replace HandleCommand instead and add a message for too many args
        __result = consumed == input.Count
            && parsedParameters != null
            && parsedParameters.All(IsParameterParsedOK);
        return false;
    }

    private static bool IsParameterParsedOK(object o)
    {
        return o is Array arr
            ? arr.Length > 0
            : o != Type.Missing;
    }

    [HarmonyPatch(typeof(ConsoleCommandsPatcher), nameof(ConsoleCommandsPatcher.HandleCommand))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> BetterTypeNames1(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher matcher = new(instructions);
        matcher.MatchForward(true,
            new CodeMatch(OpCodes.Ldloc_2),
            new CodeMatch(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo { Name: "get_ParameterTypes" }),
            new CodeMatch(ci => ci.opcode == OpCodes.Ldloc_S && ci.operand is LocalBuilder { LocalIndex: 8 }),
            new CodeMatch(OpCodes.Ldelem_Ref),
            new CodeMatch(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo { Name: "get_Name" })
        );
        if (matcher.IsValid)
        {
            matcher.Operand = new Func<Type, string>(GetAliasedTypeName).Method;
        }

        return matcher.InstructionEnumeration();
    }

    [HarmonyPatch(typeof(ConsoleCommandsPatcher), "GetColoredString", typeof(Parameter))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> BetterTypeNames2(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher matcher = new(instructions);
        matcher.MatchForward(true,
            new CodeMatch(ci => ci.opcode == OpCodes.Call && ci.operand is MethodInfo { Name: "get_ParameterType" }),
            new CodeMatch(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo { Name: "get_Name" })
        );
        if (matcher.IsValid)
        {
            matcher.Operand = new Func<Type, string>(GetAliasedTypeName).Method;
        }

        return matcher.InstructionEnumeration();
    }

    private static readonly List<string> _buildinTypeAliases =
    [
        "void",
        null,   // all other types
        "DBNull",
        "bool",
        "char",
        "sbyte",
        "byte",
        "short",
        "ushort",
        "int",
        "uint",
        "long",
        "ulong",
        "float",
        "double",
        "decimal",
        null,   // DateTime?
        null,   // ???
        "string"
    ];

    public static string GetAliasedTypeName(this Type type)
    {
        if (type.IsArray)
            return GetAliasedTypeName(type.GetElementType()) + "[]";
        if (Nullable.GetUnderlyingType(type) is { } actualType)
            return GetAliasedTypeName(actualType) + "?";
        return _buildinTypeAliases[(int) Type.GetTypeCode(type)] ?? type.Name;
    }

    private static int BetterTryParseParameters(List<string> input, List<Parameter> parameters, out object[] parsedParameters)
    {
        parsedParameters = null;
        int inputCount = input.Count;

        int paramCount = parameters.Count;
        int optionalCount = parameters.Count(param => param.IsOptional);

        if (parameters[^1].ParameterType.IsArray)
            optionalCount++;
        int requiredCount = paramCount - optionalCount;

        if (requiredCount > inputCount) return 0;

        parsedParameters = new object[paramCount];
        for (int i = 0; i < parsedParameters.Length; i++)
        {
            Type paramType = parameters[i].ParameterType;
            parsedParameters[i] = paramType.IsArray
                ? Array.CreateInstance(paramType.GetElementType(), Math.Max(0, inputCount - i))
                : Type.Missing;
        }

        int consumed = 0;
        int parsed = 0;
        while (consumed < inputCount)
        {
            // too many inputs, abort
            if (parsed >= parameters.Count) break;

            Parameter parameter = parameters[parsed];

            string inputItem = input[consumed];

            object parsedItem;
            try
            {
                parsedItem = parameter.Parse(inputItem);
                consumed++;
            }
            catch (Exception)
            {
                // TODO: need to change HandleCommand to report parse errors for arrays
                // (currently prints " is not a valid !")
                break;
            }

            if (parameter.ParameterType.IsArray)
            {
                int arrLength = inputCount - parsed;
                Array parsedArr = parsedParameters[parsed] as Array;
                bool arrayExists = parsedArr is { };
                bool arrayHasEnoughCapacity = arrayExists && parsedArr.Length >= arrLength;
                if (!arrayHasEnoughCapacity)
                {
                    // arrays always consume all available parameters
                    Array newParsedArr = Array.CreateInstance(parameter.ParameterType.GetElementType(), arrLength);
                    if (arrayExists)
                        parsedArr.CopyTo(newParsedArr, 0);

                    parsedParameters[parsed] = newParsedArr;
                }
                parsedArr.SetValue(parsedItem, consumed - 1 - parsed);
                // this theoretically shouldn't matter but let's do it anyway
                if (consumed >= inputCount) parsed++;
            }
            else
            {
                parsedParameters[parsed] = parsedItem;
                parsed++;
            }
        }
        return consumed;
    }
}
