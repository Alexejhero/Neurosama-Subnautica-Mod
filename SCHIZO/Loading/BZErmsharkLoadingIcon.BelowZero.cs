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
        // indices correspond to <see cref="uGUI_SceneLoading.State"/>
        ourAnimations = [idle, moving, stopping];
        rectTransform = loadingScreen.pengling.rectTransform;

        originalAnimations = loadingScreen.animations;
        originalTexture = loadingScreen.pengling.mainTexture;
        originalTextureWidth = loadingScreen.textureWidth;
        originalSpeedCurve = loadingScreen.speedCurve;
        originalRows = loadingScreen.rows;
        originalCols = loadingScreen.cols;

        SaveUtils.RegisterOnStartLoadingEvent(OnLoading);
    }

    private void OnDestroy()
    {
        SaveUtils.UnregisterOnStartLoadingEvent(OnLoading);
    }

    private void OnLoading()
    {
        if (Utils.GetContinueMode())
            SetOurs();
        else
            SetOriginal();
    }

    private void SetOurs()
    {
        if (isOurs) return;
        if (!loadingScreen) loadingScreen = GetComponentInParent<uGUI_SceneLoading>();

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
        if (!loadingScreen) loadingScreen = GetComponentInParent<uGUI_SceneLoading>();

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

#if DEBUG_LOADING_SCREEN
    public void Update()
    {
        if (GameModeManager.gameModePresets is not { Count: > 0 })
            return;
        if (GameModeManager.gameOptionsManager is null)
            GameModeManager.SetGameOptions(GameModePresetId.Custom);
        loadingScreen.debug = true;
        SetOurs();
        bool show = !Input.GetKey(KeyCode.LeftShift);
        loadingScreen.loadingBackground.SetState(show);
        loadingScreen.isLoading = show;
        if (Input.GetKey(KeyCode.RightArrow))
            loadingScreen.debugProgress += Time.unscaledDeltaTime * 0.25f;
        if (Input.GetKey(KeyCode.LeftArrow))
            loadingScreen.debugProgress -= Time.unscaledDeltaTime * 0.25f;
        loadingScreen.debugProgress = loadingScreen.debugProgress.Clamp01();
    }
#endif

    // there used to be a beautiful, fully documented transpiler here
    // but when we started to need five (5) distinct patches, i decided it was just not worth it
    [HarmonyPatch(typeof(uGUI_SceneLoading), nameof(uGUI_SceneLoading.OnUpdate))]
    [HarmonyPrefix]
    private static bool UpdateLoading(uGUI_SceneLoading __instance)
    {
        uGUI_SceneLoading @this = __instance;
        if (@this.IsLoading || @this.debug)
        {
            LoadingStage.FillStages(@this.stages);
            @this.SetProgress(@this.CalcProgress());
        }
        if (@this.loadingBackground.canvasGroup.alpha > 0f)
        {
            // our addition - see where this is used to see all the other changes
            bool isOurs = instance && instance.isOurs;

            @this.materialBar.SetFloat(ShaderPropertyID._Amount, @this.progress);
            float screenWidth = ((RectTransform) @this.pengling.rectTransform.parent).rect.width;
            float spriteWidth = @this.textureWidth / @this.cols;
            float moveThresholdOffset = -spriteWidth * (isOurs ? 0.5f : 0.8f); // our sprite is wider
            float progressHeadPos = screenWidth * @this.progress + moveThresholdOffset;
            if (@this.position > progressHeadPos)
                @this.position = progressHeadPos;
            if (@this.debug && @this.position >= progressHeadPos)
                @this.position = 0f;
            if (@this.state == uGUI_SceneLoading.State.None)
            {
                @this.position = moveThresholdOffset;
                @this.state = uGUI_SceneLoading.State.Idle;
            }
#if DEBUG_LOADING_SCREEN
            float dt = Time.deltaTime;
#else
            float dt = Time.unscaledDeltaTime;
#endif
            @this.time += dt;
            float duration = GetAnimDuration(__instance);
            float animLoopProportion = Mathf.Clamp01(@this.time / duration);
            bool hasLooped = @this.time >= duration;
            switch (@this.state)
            {
                case uGUI_SceneLoading.State.Idle:
                    if (@this.position + spriteWidth <= progressHeadPos
                        && (!isOurs || hasLooped)) // wait for loop to finish before moving
                    {
                        @this.time = 0f;
                        @this.state = uGUI_SceneLoading.State.Move;
                        if (isOurs)
                            @this.speedCurve = instance.idleToMovingSpeedCurve;
                    }
                    else
                    {
                        @this.time %= duration;
                    }

                    break;
                case uGUI_SceneLoading.State.Move:
                    {
                        if (isOurs && hasLooped) // after first animloop from idle to moving
                            @this.speedCurve = instance.movingLoopSpeedCurve;

                        float deltaPos = @this.speed * @this.speedCurve.Evaluate(animLoopProportion) * dt;
                        float margin = progressHeadPos - @this.position;
                        if (deltaPos > margin)
                        {
                            @this.position = progressHeadPos;
                            @this.time = 0f;
                            @this.state = uGUI_SceneLoading.State.Fall;
                        }
                        else
                        {
                            @this.time %= duration;
                            @this.position += deltaPos;
                        }
                        break;
                    }
                case uGUI_SceneLoading.State.Fall:
                    if (@this.time > duration)
                    {
                        @this.time = 0f;
                        @this.state = uGUI_SceneLoading.State.Idle;
                    }
                    break;
            }
            animLoopProportion = Mathf.Clamp01(@this.time / duration); // time could have changed
            BZAnimation animation = @this.animations[(int) @this.state];
            int frame = animation.from + Mathf.FloorToInt(animLoopProportion * animation.total);
            @this.materialPengling.SetVector(ShaderPropertyID._MainTex_ST, new Vector4(screenWidth / spriteWidth, 1f, -@this.position / spriteWidth, 0f));
            @this.materialPengling.SetVector(ShaderPropertyID._UVRect, MathExtensions.GetFrameScaleOffset(@this.cols, @this.rows, true, frame));
        }
        return false;
    }

    private const float _originalFramerate = 18;
    private static bool _loggedZeroFramerateWarning;
    private static float GetAnimDuration(uGUI_SceneLoading loading)
    {
        if (!instance || !instance.isOurs)
            return loading.animations[(int)loading.state].duration;

        FrameAnimation ourAnim = (int) loading.state switch
        {
            0 => instance.idle,
            1 => instance.moving,
            2 => instance.stopping,
            _ => default,
        };
        float framerate = ourAnim.framerate;
        if (ourAnim.framerate == default) // division by zero
        {
            if (!_loggedZeroFramerateWarning)
            {
                LOGGER.LogWarning($"Loading screen {loading.state switch
                {
                    uGUI_SceneLoading.State.None => "init",
                    uGUI_SceneLoading.State.Idle => "idle",
                    uGUI_SceneLoading.State.Move => "moving",
                    uGUI_SceneLoading.State.Fall => "falling",
                    _ => $"Unknown({(int)loading.state})",
                }} animation has 0 framerate - this is not allowed. Defaulting to {_originalFramerate}fps. This will only be logged once.");
                _loggedZeroFramerateWarning = true;
            }
            framerate = _originalFramerate;
        }

        return ourAnim.frameCount / framerate;
    }
}
