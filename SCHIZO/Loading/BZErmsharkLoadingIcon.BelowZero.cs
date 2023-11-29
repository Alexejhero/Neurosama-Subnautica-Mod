//#warning DEBUG
//#define DEBUG
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
    partial struct Animation
    {
        public static implicit operator BZAnimation(Animation anim)
            => new() { from = anim.from, to = anim.to };
    }

    public static BZErmsharkLoadingIcon instance;

    private uGUI_SceneLoading loadingScreen;
    private BZAnimation[] ourAnimations;
    private float[] ourFramerates;

    private BZAnimation[] originalAnimations;
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
            new CodeInstruction(OpCodes.Call, new Func<uGUI_SceneLoading, bool>(HasAnimJustLooped).Method),
            new CodeInstruction(OpCodes.Brfalse, breakLabel)
        );
        return true;
    }

    [HarmonyPatch(typeof(BZAnimation), nameof(BZAnimation.duration), MethodType.Getter)]
    [HarmonyPrefix]
    private static bool CustomIconAnimFramerate(out float __result)
    {
        __result = 0;
        if (!instance.isOurs) return true;

        // technically a sin... but get_duration is only called inside Update
        Animation ourAnim = (int)instance.loadingScreen.state switch
        {
            0 => instance.idle,
            1 => instance.moving,
            2 => instance.stopping,
            _ => default,
        };
        if (ourAnim.framerate == 0) return true;

        __result = ourAnim.frameCount / ourAnim.framerate;
        return false;
    }
    private static float _prop = 0.4f;
    private static float GetMoveThresholdProportion()
        => instance && instance.isOurs ? _prop : 0.8f;

    private static bool HasAnimJustLooped(uGUI_SceneLoading loading)
    {
        if (!instance || !instance.isOurs) return true;
        BZAnimation anim = loading.animations[(int) loading.state];
        int frame = GetCurrentFrame(anim, loading.time);

        return frame == anim.from || frame == anim.to; // leeway of one frame on the loop
    }

    private static int GetCurrentFrame(BZAnimation anim, float time)
        => anim.from + (int)((time / anim.duration) * anim.total);
}
