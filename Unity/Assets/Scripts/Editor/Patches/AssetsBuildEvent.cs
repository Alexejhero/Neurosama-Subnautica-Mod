﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using AssetBundleBrowser.AssetBundleDataSource;
using HarmonyLib;
using JetBrains.Annotations;
using SCHIZO.Unity.Items;
using SCHIZO.Unity.Utilities;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[HarmonyPatch, UsedImplicitly]
public static class AssetsBuildEvent
{
    private static readonly MethodInfo _patchMethod = AccessTools.Method(typeof(AssetsBuildEvent), nameof(Patch));

    [HarmonyTargetMethod, UsedImplicitly]
    public static MethodBase TargetMethod() => AccessTools.Method("AssetDatabaseABDataSource:BuildAssetBundles");

    [HarmonyTranspiler, UsedImplicitly]
    public static IEnumerable<CodeInstruction> Matcher(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher matcher = new CodeMatcher(instructions);
        matcher.SearchForward(i => i.opcode == OpCodes.Ret);
        matcher.InsertAndAdvance
        (
            new CodeInstruction(OpCodes.Ldarg_1),
            new CodeInstruction(OpCodes.Ldloc_0),
            new CodeInstruction(OpCodes.Call, _patchMethod)
        );
        return matcher.InstructionEnumeration();
    }

    private static void Patch(ABBuildInfo info, AssetBundleManifest manifest)
    {
        foreach (string name in manifest.GetAllAssetBundles())
        {
            string path = Path.Combine(info.outputDirectory, name);
            AssetBundle bundle = AssetBundle.LoadFromFile(path);

            List<(string, Type)> result = new List<(string, Type)>();
            foreach (string asset in bundle.GetAllAssetNames())
            {
                string capitalizedAssetName = AssetDatabase.GUIDToAssetPath(AssetDatabase.AssetPathToGUID(asset));
                UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(asset);
                UnityEngine.Object main = AssetDatabase.LoadMainAssetAtPath(asset);

                if (!asset.StartsWith("assets/")) throw new Exception("Don't know how to handle asset: " + asset);

                if (assets.Length == 1)
                {
                    result.Add((capitalizedAssetName.Substring(7), assets[0].GetType()));
                }
                else
                {
                    Object includedAsset = HandleMultipleAssets(assets, main, asset);
                    if (!includedAsset) continue;

                    result.Add((capitalizedAssetName.Substring(7), includedAsset.GetType()));
                }
            }

            File.WriteAllText(Path.Combine(info.outputDirectory, $"{GetCleanName(name)}.cs"), GetCode(name, result));

            bundle.Unload(true);
        }
    }

    private static UnityEngine.Object HandleMultipleAssets(UnityEngine.Object[] objects, UnityEngine.Object main, string path)
    {
        if (main is GameObject) return main;
        if (main is CloneItemData) return main;

        if (objects[0] is Texture2D && objects[1] is Sprite sprite1) return sprite1;
        if (objects[0] is Sprite sprite2 && objects[1] is Texture2D) return sprite2;

        throw new Exception($"Don't know how to handle multiple assets: {path} - {string.Join(", ", objects.Select(t => t.GetType().Name))}");
    }

    private static string GetCode(string className, List<(string, Type)> assets)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine
        ($@"

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was automatically generated. PLEASE DO NOT MODIFY THIS FILE MANUALLY!
// </auto-generated>
//------------------------------------------------------------------------------

// Resharper disable all

namespace SCHIZO.Resources;

public static class {GetCleanName(className)}
{{
    private const int _rnd = {Random.Range(int.MinValue, int.MaxValue)};

    private static readonly UnityEngine.AssetBundle _a = ResourceManager.GetAssetBundle(""assets"");

    public static T[] All<T>() where T : UnityEngine.Object => _a.LoadAllAssets<T>();
    public static UnityEngine.Object[] All() => _a.LoadAllAssets();
        ");

        foreach ((string asset, Type type) in assets)
        {
            builder.AppendLine($@"    public static {type.FullName} {GetCleanName(asset)} = _a.LoadAsset<{type.FullName}>(""Assets/{asset}"");");
        }

        builder.AppendLine("}");

        return builder.ToString();
    }

    private static string GetCleanName(string asset)
    {
        return new string(transform().ToArray());

        IEnumerable<char> transform()
        {
            bool upper = true;
            string path = asset.Substring(0, asset.Length - Path.GetFileName(asset).Length + Path.GetFileNameWithoutExtension(asset).Length);//Path.GetFileNameWithoutExtension(asset);
            foreach (char c in path)
            {
                if (" ()_-,.".Contains(c))
                {
                    upper = true;
                }
                else if ("/\\".Contains(c))
                {
                    upper = true;
                    yield return '_';
                }
                else if ("0123456789".Contains(c))
                {
                    upper = true;
                    yield return c;
                }
                else if (upper)
                {
                    upper = false;
                    yield return char.ToUpper(c);
                }
                else
                {
                    yield return c;
                }
            }
        }
    }
}
