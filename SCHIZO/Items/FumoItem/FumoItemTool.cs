using System.Collections;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Items.FumoItem;

partial class FumoItemTool
{
    public bool IsHugging => _isHugging;
    public bool IsAltEffectActive => isAltEffectActive;
    private Transform FumoModel => RetargetHelpers.Pick(fumoModelSN, fumoModelBZ);

    private const float _hugTransitionDuration = 0.2f;
    private float _hugDistScale;
    private float _nextHugTime;
    private readonly Vector3 _chestOffset = new(0, -0.1f, 0);

    private bool _isHugging;
    private float _hugTime;
    private const float _hugDistance = 0.5f;
    private Vector3 _prevHugPosOffset;
    private bool _hugEffectApplied;
    private const float _hugMoveSpeedMulti = 0.7f;

    protected bool altUseEnabled;
    protected bool _altEffectOnHug;
    protected bool isAltEffectActive;
    private float _altEffectTimeRemaining;
    private float _flushedZscalar = 1.5f;

    private GroundMotor _groundMotor;

    public new void Awake()
    {
        if (hasAltUse)
        {
            altUseEnabled = Random.Range(0f, 1f) < altUsableChance;
            hasAltUse = altUseEnabled;
            _altEffectOnHug = !altUseEnabled && Random.Range(0f, 1f) < altEffectOnHugChance;
        }

        base.Awake();
    }

    public void FixedUpdate()
    {
        if (!usingPlayer) return;

        if (_altEffectOnHug)
        {
            if (_isHugging)
            {
                _hugTime += Time.fixedDeltaTime;
                if (_hugTime > altEffectMinHugTime) SetAltEffect(true);
            }
            else
            {
                _hugTime = 0;
            }
        }
        _altEffectTimeRemaining -= Time.fixedDeltaTime;
        if (_altEffectTimeRemaining < 0f) SetAltEffect(false);
    }

    public void Update()
    {
        if (!usingPlayer) return;

        float delta = Time.deltaTime / _hugTransitionDuration;
        _hugDistScale = _isHugging
            ? Mathf.Min(1, _hugDistScale + delta)
            : Mathf.Max(0, _hugDistScale - delta);

        UpdateHugPos(_hugDistScale);
    }

    private void UpdateHugPos(float distScale)
    {
        (Transform parent, Vector3 offset) = GetHugOffset(distScale);

        Vector3 delta = offset - _prevHugPosOffset;
        parent.localPosition += delta;
        _prevHugPosOffset = offset;
    }

    public override void OnDraw(Player p)
    {
        p.isUnderwater.changedEvent.AddHandler(this, GroundSpeedHack);
        base.OnDraw(p);
    }

    public override void OnHolster()
    {
        usingPlayer.isUnderwater.changedEvent.RemoveHandler(this, GroundSpeedHack);
        // need to reset immediately, otherwise PDA opens in the wrong location
        UpdateHugPos(0);
        StopHugging();
        if (isAltEffectActive) SetAltEffect(false);
        base.OnHolster();
    }

    public override bool OnRightHandDown() => false; // don't play floater release anim

    public override bool OnRightHandHeld()
    {
        if (!_isHugging && Time.time > _nextHugTime)
            StartHugging();
        return base.OnRightHandHeld();
    }

    public override bool OnRightHandUp()
    {
        StopHugging();
        return base.OnRightHandUp();
    }

    public override bool OnAltHeld()
    {
        if (!altUseEnabled) return false;

        return SetAltEffect(true) && base.OnAltHeld();
    }

    public void StartHugging()
    {
        if (_isHugging || !usingPlayer) return;
        _isHugging = true;

        if (_hugEffectApplied) return;
        ApplyMoveSpeedMulti(_hugMoveSpeedMulti);
        ApplyColdResistBuff(hugColdResistBuff);
        _hugEffectApplied = true;
    }

    public void StopHugging()
    {
        if (!_isHugging || !usingPlayer) return;
        _isHugging = false;
        _nextHugTime = Time.time + hugCooldown;

        if (!_hugEffectApplied) return;
        ApplyMoveSpeedMulti(1f/_hugMoveSpeedMulti);
        ApplyColdResistBuff(-hugColdResistBuff);
        _hugEffectApplied = false;
    }

    // a formal apology for the following code may be issued to any Subnautica dev or modder on request
    private void ApplyMoveSpeedMulti(float multi)
    {
        Player player = usingPlayer ? usingPlayer : Player.main;
        if (!player) return;
        if (!_groundMotor) _groundMotor = player.GetComponent<GroundMotor>();

        ApplyGroundMoveSpeedMulti(multi);
        // this is fortunately enough to change underwater swim speed
        _groundMotor.GetComponent<UnderwaterMotor>().debugSpeedMult *= multi;
    }

    private void GroundSpeedHack(Utils.MonitoredValue<bool> isUnderwater)
    {
        if (!_isHugging || isUnderwater.value) return;
        StartCoroutine(GroundSpeedHackCoro(_hugMoveSpeedMulti));

        IEnumerator GroundSpeedHackCoro(float multi)
        {
            yield return null;
            ApplyGroundMoveSpeedMulti(multi);
        }
    }

    private void ApplyGroundMoveSpeedMulti(float multi)
    {
        _groundMotor.forwardMaxSpeed *= multi;
        _groundMotor.strafeMaxSpeed *= multi;
        _groundMotor.backwardMaxSpeed *= multi;
    }

    private bool SetAltEffect(bool active)
    {
        if (active) _altEffectTimeRemaining = altEffectDuration;
        if (isAltEffectActive == active) return false;

        isAltEffectActive = active;
        ApplyAltEffect(active);
        return true;
    }

    protected virtual void ApplyAltEffect(bool active)
    {
        ApplyZScaleMulti(active ? _flushedZscalar : 1 / _flushedZscalar);
    }

    private void ApplyZScaleMulti(float multi)
    {
        Vector3 scale = FumoModel.localScale;
        scale.z *= multi;
        FumoModel.localScale = scale;
    }
}
