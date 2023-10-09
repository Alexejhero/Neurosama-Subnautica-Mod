using UnityEngine;

namespace SCHIZO.Items.FumoItem;

public sealed partial class FumoItemTool : CustomPlayerTool
{
    private const float hugTransitionDuration = 0.2f;
    private float hugDistScale;
    private const float hugCooldown = 1f;
    private float nextHugTime;
    private readonly Vector3 chestOffset = new(0, -0.3f, 0);

    private bool isHugging;
    private const float hugDistance = 0.5f;
    private Vector3 prevHugPosOffset;
    private const float hugMoveSpeedMulti = 0.7f;
    private const int hugColdResistBuff = 20;
    private bool hugEffectApplied;

    private bool canFlush;
    private bool isFlushed;
    private const float flushedZscalar = 2f;

    private GroundMotor groundMotor;

    public new void Awake()
    {
        animTechType = TechType.Floater;
        hasPrimaryUse = true;
        primaryUseTextLanguageString = "Hug ({0})";
        holsterTime = 0.1f;

        canFlush = Random.Range(0f, 1f) < 0.05f;
        if (canFlush)
        {
            hasAltUse = true;
            altUseTextLanguageString = "Flushed ({0})";
        }

        base.Awake();
    }

    public void Update()
    {
        if (!usingPlayer) return;

        float delta = Time.deltaTime / hugTransitionDuration;
        hugDistScale = isHugging
            ? Mathf.Min(1, hugDistScale + delta)
            : Mathf.Max(0, hugDistScale - delta);

        UpdateHugPos(hugDistScale);
    }

    private void UpdateHugPos(float distScale)
    {
        (Transform parent, Vector3 offset) = GetHugOffset(distScale);

        Vector3 delta = offset - prevHugPosOffset;
        parent.localPosition += delta;
        prevHugPosOffset = offset;
    }

    public override void OnHolster()
    {
        // need to reset immediately, otherwise PDA opens in the wrong location
        UpdateHugPos(0);
        StopHugging();
        if (isFlushed) ApplyZScaleMulti(1f / flushedZscalar);
        base.OnHolster();
    }

    public override bool OnRightHandDown() => false; // don't play floater release anim

    public override bool OnRightHandHeld()
    {
        if (!isHugging && Time.time > nextHugTime)
            StartHugging();
        return base.OnRightHandHeld();
    }

    public override bool OnRightHandUp()
    {
        StopHugging();
        return base.OnRightHandUp();
    }

    public override bool OnAltDown()
    {
        if (!canFlush || isFlushed) return false;

        isFlushed = true;
        ApplyZScaleMulti(flushedZscalar);
        return base.OnAltDown();
    }

    public override bool OnAltUp()
    {
        if (!canFlush || !isFlushed) return false;

        isFlushed = false;
        ApplyZScaleMulti(1f / flushedZscalar);
        return base.OnAltUp();
    }

    public void StartHugging()
    {
        if (isHugging || !usingPlayer) return;
        isHugging = true;

        if (hugEffectApplied) return;
        ApplyMoveSpeedMulti(hugMoveSpeedMulti);
        ApplyColdResistBuff(hugColdResistBuff);
        hugEffectApplied = true;
    }

    public void StopHugging()
    {
        if (!isHugging || !usingPlayer) return;
        isHugging = false;
        nextHugTime = Time.time + hugCooldown;

        if (!hugEffectApplied) return;
        ApplyMoveSpeedMulti(1f/hugMoveSpeedMulti);
        ApplyColdResistBuff(-hugColdResistBuff);
        hugEffectApplied = false;
    }

    private void ApplyMoveSpeedMulti(float multi)
    {
        // a formal apology for the following lines of code can be issued to any Subnautica dev or modder on request
        Player player = usingPlayer ? usingPlayer : Player.main;
        if (!player) return;
        if (!groundMotor) groundMotor = player.GetComponent<GroundMotor>();
        groundMotor.forwardMaxSpeed *= multi;
        groundMotor.strafeMaxSpeed *= multi;
        groundMotor.backwardMaxSpeed *= multi;
        groundMotor.GetComponent<UnderwaterMotor>().debugSpeedMult *= multi;
    }

    private void ApplyZScaleMulti(float multi)
    {
        Transform vm = transform.Find("VM/neurofumo new");
        Vector3 scale = vm.localScale;
        scale.z *= multi;
        vm.localScale = scale;
    }
}
