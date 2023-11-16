using System.Collections;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Items.FumoItem;

partial class FumoItemTool
{
    public bool IsHugging => _isHugging;
    public bool IsFlushed => _isFlushed;
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

    private bool _flushOnAltUse;
    private bool _flushOnHug;
    private bool _isFlushed;
    private float _flushedZscalar = 1.5f;

    private GroundMotor _groundMotor;

    public new void Awake()
    {
        _flushOnAltUse = Random.Range(0f, 1f) < 0.05f;
        hasAltUse = _flushOnAltUse;
        _flushOnHug = !_flushOnAltUse && Random.Range(0f, 1f) < 0.5f;

        base.Awake();
    }

    public void FixedUpdate()
    {
        if (!usingPlayer) return;
        if (!_flushOnHug) return;

        if (_isHugging)
        {
            _hugTime += Time.fixedDeltaTime;
            if (_hugTime > 10f) SetFlushed(true);
        }
        else
        {
            _hugTime = 0;
            if (_hugDistScale == 0f) SetFlushed(false);
        }
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
        if (_isFlushed) SetFlushed(false);
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

    public override bool OnAltDown()
    {
        if (!_flushOnAltUse) return false;

        return SetFlushed(true) && base.OnAltDown();
    }

    public override bool OnAltUp()
    {
        if (!_flushOnAltUse) return false;

        return SetFlushed(false) && base.OnAltUp();
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

    public bool SetFlushed(bool flushed)
    {
        if (_isFlushed == flushed) return false;

        _isFlushed = flushed;
        ApplyZScaleMulti(flushed ? _flushedZscalar : 1 / _flushedZscalar);
        return true;
    }

    private void ApplyZScaleMulti(float multi)
    {
        Vector3 scale = FumoModel.localScale;
        scale.z *= multi;
        FumoModel.localScale = scale;
    }
}
