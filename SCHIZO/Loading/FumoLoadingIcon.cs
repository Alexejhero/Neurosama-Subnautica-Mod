using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace SCHIZO.Loading;

partial class FumoLoadingIcon
{
    private static bool _patched;

    private uGUI_Logo _logo;
    private Texture2D _originalTexture;
    private Action _onEnable;

    private void Awake()
    {
        _logo = transform.parent.GetComponent<uGUI_Logo>();
        _originalTexture = _logo.texture;
        _onEnable = AccessTools.MethodDelegate<Action>(AccessTools.Method(typeof(Graphic), "OnEnable"), this);

        if (!_patched)
        {
            _patched = true;
            HARMONY.Patch(AccessTools.Method(typeof(uGUI_Logo), nameof(uGUI_Logo.Update)),
                transpiler: new HarmonyMethod(AccessTools.Method(typeof(FumoLoadingIcon), nameof(FixFumoRotationPatch))));
        }
    }

    private void Update()
    {
        bool textureIsOriginal = _logo.texture == _originalTexture;
        bool shouldShowFumo = ShouldShowFumo();

        if (textureIsOriginal && shouldShowFumo)
        {
            _logo.texture = texture;
            TriggerMeshUpdate();
        }
        else if (!textureIsOriginal && !shouldShowFumo)
        {
            _logo.texture = _originalTexture;
            TriggerMeshUpdate();
        }
    }

    private bool ShouldShowFumo()
    {
        if (GetComponentInParent<uGUI_IntroTab>()) return false; // Don't override the logo on the PDA boot animation
        if (GetComponentInParent<uGUI_SceneLoading>() && !Utils.GetContinueMode()) return false; // Don't override the loading icon when starting a new game
        return true;
    }

    private void TriggerMeshUpdate()
    {
        _onEnable();
    }

    private static IEnumerable<CodeInstruction> FixFumoRotationPatch(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher matcher = new(instructions);
        matcher.Start();
        matcher.Advance(1);
        matcher.SearchForward(i => i.IsLdarg(0));

        int labelPos = matcher.Pos;
        matcher.InsertAndAdvance
        (
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldloc_0),
            new CodeInstruction(OpCodes.Call, new Action<uGUI_Logo, float>(Reroute).Method)
        );

        matcher.AddLabelsAt(labelPos, matcher.Labels);
        matcher.RemoveInstructions(11);
        return matcher.InstructionEnumeration();

        static void Reroute(uGUI_Logo __instance, float unscaledDeltaTime)
        {
            int maxAngle = __instance.GetComponentInChildren<FumoLoadingIcon>().ShouldShowFumo() ? 360 : 180;
            __instance.angle = (__instance.angle + __instance.rotationSpeed * unscaledDeltaTime) % maxAngle;
        }
    }
}
