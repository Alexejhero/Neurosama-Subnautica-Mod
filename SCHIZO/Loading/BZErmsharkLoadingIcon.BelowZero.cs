using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Nautilus.Utility;
using UnityEngine;
using BZAnimation = uGUI_SceneLoading.Animation;

namespace SCHIZO.Loading;

[HarmonyPatch]
partial class BZErmsharkLoadingIcon
{
    partial struct FrameAnimation
    {
        public static implicit operator BZAnimation(FrameAnimation anim)
            => new() { from = anim.from, to = anim.to };
    }

    public static BZErmsharkLoadingIcon instance;

    private uGUI_SceneLoading loadingScreen;
    private BZAnimation[] ourAnimations;
    private float[] ourFramerates;

    private BZAnimation[] originalAnimations;
    private AnimationCurve originalSpeedCurve;
    private Texture originalTexture;
    private float originalTextureWidth;
    private int originalRows;
    private int originalCols;

    private RectTransform rectTransform;
    private RectTransform rectParent;

    private Vector2 originalSpriteDimensions => new(originalTexture.width / originalCols, originalTexture.height / originalRows);
    private Vector2 ourSpriteDimensions => new(texture.width / columns, texture.height / rows);

    private bool isOurs;

    private void Awake()
    {
        instance = this;
        loadingScreen = GetComponentInParent<uGUI_SceneLoading>();
        if (!loadingScreen)
        {
            Destroy(this);
            return;
        }
        /// these correspond to the <see cref="uGUI_SceneLoading.State"/>
        ourAnimations = [idle, moving, stopping];
        rectTransform = loadingScreen.pengling.rectTransform;
        rectParent = (RectTransform) rectTransform.parent;

        originalAnimations = loadingScreen.animations;
        originalTexture = loadingScreen.pengling.mainTexture;
        originalTextureWidth = loadingScreen.textureWidth;
        originalSpeedCurve = loadingScreen.speedCurve;
        originalRows = loadingScreen.rows;
        originalCols = loadingScreen.cols;

        SaveUtils.RegisterOnStartLoadingEvent(() =>
        {
            if (Utils.GetContinueMode())
                SetOurs();
            else
                SetOriginal();
        });
    }

    private void SetOurs()
    {
        if (isOurs) return;

        loadingScreen.materialPengling.mainTexture = texture;
        loadingScreen.pengling.texture = texture;
        loadingScreen.textureWidth = texture.width;
        loadingScreen.speedCurve = movingLoopSpeedCurve;
        rectTransform.sizeDelta = new Vector2(0, ourSpriteDimensions.y);
        loadingScreen.rows = rows;
        loadingScreen.cols = columns;
        loadingScreen.animations = ourAnimations;

        isOurs = true;
    }

    private void SetOriginal()
    {
        if (!isOurs) return;

        loadingScreen.materialPengling.mainTexture = originalTexture;
        loadingScreen.pengling.texture = originalTexture;
        loadingScreen.textureWidth = originalTextureWidth;
        loadingScreen.speedCurve = originalSpeedCurve;
        rectTransform.sizeDelta = new Vector2(0, originalSpriteDimensions.y);
        loadingScreen.rows = originalRows;
        loadingScreen.cols = originalCols;
        loadingScreen.animations = originalAnimations;

        isOurs = false;
    }

#if DEBUG
    public void Update()
    {
        if (GameModeManager.gameModePresets is not { Count: > 0 })
            return;
        if (GameModeManager.gameOptionsManager is null)
            GameModeManager.SetGameOptions(GameModePresetId.Custom);
        loadingScreen.debug = true;
        SetOurs();
        bool l = !Input.GetKey(KeyCode.LeftShift);
        loadingScreen.loadingBackground.SetState(l);
        loadingScreen.isLoading = l;
        if (Input.GetKey(KeyCode.RightArrow))
            loadingScreen.debugProgress += Time.unscaledDeltaTime * 0.25f;
        if (Input.GetKey(KeyCode.LeftArrow))
            loadingScreen.debugProgress -= Time.unscaledDeltaTime * 0.25f;
        loadingScreen.debugProgress = loadingScreen.debugProgress.Clamp01();
    }
#endif

    [HarmonyPatch(typeof(uGUI_SceneLoading), nameof(uGUI_SceneLoading.OnUpdate))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> UpdateOurIconProperly(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher matcher = new(instructions);
        if (!Patch1_SmallerThresholdToStartMoving(matcher))
            LOGGER.LogFatal("Could not apply patch 1 (smaller idle->moving gap) to uGUI_SceneLoading.OnUpdate!");

        matcher.Start(); // reset in case the patch failed
        if (!Patch2_WaitForIdleLoopToStartMoving(matcher))
            LOGGER.LogFatal("Could not apply patch 2 (wait for idle anim loop before moving) to uGUI_SceneLoading.OnUpdate!");

        matcher.Start();
        if (!Patch3_CheckSpeedCurveOnMovingLoop(matcher))
            LOGGER.LogFatal("Could not apply patch 3 (moving loop speed curve) to uGUI_SceneLoading.OnUpdate!");

        return matcher.InstructionEnumeration();
    }

    private static bool Patch1_SmallerThresholdToStartMoving(CodeMatcher matcher)
    {
        // patch 1 - smaller gap to start moving from idle (it's a proportion of the sprite width)
        matcher.MatchForward(true,
            new CodeMatch(OpCodes.Ldc_R4, 0.8f)
        );
        if (!matcher.IsValid) return false;
        
        matcher.Set(OpCodes.Call, new Func<float>(GetMoveThresholdProportion).Method);
        return true;
    }

    private static bool Patch2_WaitForIdleLoopToStartMoving(CodeMatcher matcher)
    {
        // patch 2 - wait for idle animation to loop before transitioning to moving
        matcher.MatchForward(true,
            // if (moveThreshold > this.position + spriteWidth)
            new CodeMatch(OpCodes.Ldloc_3),
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(uGUI_SceneLoading), nameof(uGUI_SceneLoading.position))), // we technically don't need the FieldInfo
            new CodeMatch(OpCodes.Ldloc_1),
            new CodeMatch(OpCodes.Add),
            new CodeMatch(OpCodes.Ble_Un)
        );
        if (!matcher.IsValid) return false;
        
        Label breakLabel = (Label) matcher.Operand;
        matcher.Advance(1);
        matcher.InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, new Func<uGUI_SceneLoading, bool>(CallHasAnimJustLooped).Method),
            new CodeInstruction(OpCodes.Brfalse, breakLabel)
        );
        return true;
    }

    private static bool Patch3_CheckSpeedCurveOnMovingLoop(CodeMatcher matcher)
    {
        // patch 3 - call the "check loop" function at the start of the "Move" state branch of the switch statement
        // so we can restore the normal speed curve from the animation workaround
        matcher.MatchForward(false,
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(uGUI_SceneLoading), nameof(uGUI_SceneLoading.speed)))
        );
        if (!matcher.IsValid) return false;

        matcher.Insert(
            new CodeInstruction(OpCodes.Ldarg_0)
                .MoveLabelsFrom(matcher.Instruction),
            new CodeInstruction(OpCodes.Call, new Func<uGUI_SceneLoading, bool>(CallHasAnimJustLooped).Method),
            new CodeInstruction(OpCodes.Pop)
        );
        return true;
    }

    [HarmonyPatch(typeof(BZAnimation), nameof(BZAnimation.duration), MethodType.Getter)]
    [HarmonyPrefix]
    private static bool CustomIconAnimFramerate(out float __result)
    {
        __result = 0;
        if (!instance || !instance.isOurs) return true;

        // technically a sin... but get_duration is only ever called inside Update
        FrameAnimation ourAnim = (int)instance.loadingScreen.state switch
        {
            0 => instance.idle,
            1 => instance.moving,
            2 => instance.stopping,
            _ => default,
        };
        if (ourAnim.framerate == default) return true;

        __result = ourAnim.frameCount / ourAnim.framerate;
        return false;
    }
    private static float _prop = 0.4f;
    private static float GetMoveThresholdProportion()
        => instance && instance.isOurs ? _prop : 0.8f;

    private static bool CallHasAnimJustLooped(uGUI_SceneLoading loading)
    {
        if (!instance) return true;

        bool looped = instance.HasAnimJustLooped(loading);
        if (looped && instance.isOurs)
        {
            // awful awful code
            loading.speedCurve = loading.state == uGUI_SceneLoading.State.Idle
                ? instance.idleToMovingSpeedCurve
                : instance.movingLoopSpeedCurve;
        }
        return looped;
    }

    private int _lastFrame;
    private bool HasAnimJustLooped(uGUI_SceneLoading loading)
    {
        if (!isOurs) return true;

        BZAnimation anim = loading.animations[(int) loading.state];
        int frame = GetCurrentFrame(anim, loading.time);
        bool hasLooped = frame < _lastFrame && _lastFrame <= anim.to;
        _lastFrame = frame;

        return hasLooped;
    }

    private static int GetCurrentFrame(BZAnimation anim, float time)
        => anim.from + (int)((time / anim.duration) * (anim.to - anim.from));
}
