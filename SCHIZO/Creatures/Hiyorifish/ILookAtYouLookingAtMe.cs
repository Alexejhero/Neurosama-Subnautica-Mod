using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using RuntimeDebugDraw;
using SCHIZO.Helpers;
using UnityEngine;
using UWEXR;

namespace SCHIZO.Creatures.Hiyorifish;

partial class ILookAtYouLookingAtMe : IOnTakeDamage
{
    [UsedImplicitly] // do not remove, do not make const or readonly, do not do anything
    internal bool debug = false;

    // this turned out to be a pretty "scripted" creature
    // i hope it doesn't end up feeling too heavy handed in-game
    private bool _isAggressive;
    private float _timeAlive;
    private float _timeInCurrentMode;
    private float _minDist;
    private bool _isObscured;
    private LayerMask _noPlayerLayer;
    private RaycastHit[] _hits;

    private float CurrentModeDuration => _isAggressive ? aggressiveDuration : passiveDuration;
    public bool ShouldSwapMode => _timeInCurrentMode > CurrentModeDuration && _minDist < 15;
    public float LookTurnPower => _isAggressive ? -lookTurnPower : lookTurnPower;
    private SwimBehaviour SwimBehaviour => (SwimBehaviour) swimBehaviour;

    private void Start()
    {
        // the actual renderers are on child objects somewhere and won't call OnBecameInvisible
        // we have to have a renderer on the same object as this component to get visibility checks
        gameObject.EnsureComponent<BillboardRenderer>();
        _noPlayerLayer = ~(1 << LayerMask.NameToLayer("Player"));
    }

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        _timeInCurrentMode += dt;
        _timeAlive += dt;
        if (!Player.main) return;

        Vector3 camPos = MainCameraControl.main.transform.position;
        Vector3 lineOfSight = transform.position - camPos;
        int numHits = UWE.Utils.RaycastIntoSharedBuffer(camPos,
            lineOfSight.normalized, lineOfSight.magnitude,
            Physics.AllLayers, QueryTriggerInteraction.Ignore);
        IEnumerable<RaycastHit> obscuringHits = UWE.Utils.sharedHitBuffer.Take(numHits)
            .Where(hit =>
            {
                GameObject hitObj = hit.collider.attachedRigidbody!?.gameObject
                    ?? hit.collider.gameObject;
                return hitObj != gameObject && hitObj != Player.main.gameObject;
            });
        _isObscured = obscuringHits.Any();
        if (debug && !Input.GetKey(KeyCode.LeftAlt))
        {
            _hits = obscuringHits.ToArray();
            _hits.ForEach(hit =>
            {
                Draw.DrawText(hit.point, hit.collider.gameObject.name, Color.yellow, 16, dt, false);
                Draw.DrawCube(hit.collider.bounds.min, hit.collider.bounds.max, Color.yellow, dt);
                Draw.DrawRay(hit.point, hit.normal, Color.blue, dt);
            });
        }
    }

    private void OnEnable()
    {
        // we need to schedule our update before the camera
        //ManagedUpdate.Subscribe(ManagedUpdate.Queue.UpdateCameraTransform, OnUpdate);
        ManagedUpdate.main.subscribers
            .GetOrAddNew(ManagedUpdate.Queue.UpdateCameraTransform)
            .Insert(0, OnUpdate);
    }

    private void OnDisable()
    {
        ManagedUpdate.Unsubscribe(ManagedUpdate.Queue.UpdateCameraTransform, OnUpdate);
    }

    private void OnUpdate()
    {
        if (debug && !Input.GetKey(KeyCode.LeftAlt))
        {
            Draw.DrawLine(MainCameraControl.main.transform.position, transform.position, _isObscured ? Color.red : Color.green);
        }

        UpdatePlayerCamera();

        if (ShouldSwapMode || (debug && Input.GetKeyDown(KeyCode.M)))
        {
            SwapMode();
        }
    }

    private void UpdatePlayerCamera()
    {
        if (!enabled || !Player.main || _isObscured) return;

        MainCameraControl cam = MainCameraControl.main;
        Vector3 plrToMe = transform.position - cam.transform.position;
        float distance = plrToMe.magnitude;
        Vector3 direction = plrToMe.normalized;
        if (distance > maxRange) return;
        _minDist = Mathf.Min(_minDist, distance);

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Quaternion lookErrorQ = Quaternion.Inverse(cam.rotation) * targetRotation;
        Vector2 lookError = lookErrorQ.eulerAngles.FixAngles();

        // don't try dot product we need fov
        float angle = lookError.magnitude;
        float camFov = MainCamera.camera.fieldOfView;
        // center not in view
        if (angle > camFov) return;
        // now technically camFov is the horizontal FOV and you'll get pulled/pushed even when it's offscreen vertically
        // however,

        float distanceScalar = Mathf.Pow(1 - distance / maxRange, 2);
        float turnSpeed = (camFov - angle) * distanceScalar * LookTurnPower;
        Vector2 turn = lookError.normalized * turnSpeed * Time.deltaTime;
        // kill overshoot, it results in jitter
        if (LookTurnPower > 0 && turn.magnitude > angle)
            turn = lookError;

        if (debug && !Input.GetKey(KeyCode.LeftAlt))
        {
            Vector3 lookWorldPos = MainCamera.camera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, MainCamera.camera.nearClipPlane));
            Draw.DrawLine(transform.position, lookWorldPos, Color.cyan, depthTest: false);
            if (Input.GetKey(KeyCode.LeftControl))
            {
                Draw.DrawText(transform.position, string.Join("\n", [
                    _timeAlive,
                    _isAggressive,
                    _timeInCurrentMode,
                    _minDist,
                    ShouldSwapMode,
                ]), popUp: false);
            }
            if (Input.GetKey(KeyCode.RightAlt))
                Debugger.Break();
            return;
        }

        // vomit now
        if (XRSettings.enabled) return;
        // rotationX/Y is yaw/pitch, as god intended (Vector x/y is pitch/yaw)
        cam.rotationX += turn.y;
        cam.rotationY -= turn.x;
    }

    private void OnBecameInvisible()
    {
        if (_timeAlive > vanishMinLifetime && Random.Range(0f, 1f) < vanishChance)
        {
            Destroy(gameObject); // where did it go :o
        }
    }

    // nothin personnel, kid
    private IEnumerator TeleportsBehindYou()
    {
        Player player = Player.main;
        if (!player || player.currentSub || !player.IsUnderwaterForSwimming()) yield break;
#if BELOWZERO
        if (player.currentInterior is { }) yield break;
#endif

        // pick a point behind the player
        Vector3 playerPos = player.transform.position;
        float dist = Mathf.Clamp((playerPos - transform.position).magnitude, 5, 15);
        Vector3 behindPlayer = playerPos + dist * (Random.insideUnitSphere - MainCamera.camera.transform.forward);
        if (behindPlayer.y > 0) behindPlayer.y = 0; // fish can't walk dummy
        // let's see if we hit anything
        Vector3 ray = behindPlayer - playerPos;
        int layerMask = Physics.DefaultRaycastLayers & _noPlayerLayer;
        bool didHit = Physics.Raycast(playerPos, ray.normalized, out RaycastHit hit, ray.magnitude, layerMask, QueryTriggerInteraction.Ignore);
        Vector3 closestPoint = didHit ? hit.point : behindPlayer;

        // todo some sort of vfx
        const float moveDuration = 0.1f;
        Vector3 oldPos = transform.position;
        for (float t = 0; t < moveDuration; t += Time.deltaTime)
        {
            transform.position = Vector3.Slerp(oldPos, closestPoint, t / moveDuration);
            yield return null;
        }
    }

    private void SwapMode() => SetMode(!_isAggressive);

    private void SetMode(bool aggressive)
    {
        _isAggressive = aggressive;
        _timeInCurrentMode = 0;
        _minDist = float.PositiveInfinity;

        if (SwimBehaviour) SwimBehaviour.LookForward();
        if (aggressive) StartCoroutine(AggressiveMode());
    }

    private IEnumerator AggressiveMode()
    {
        while (_isAggressive)
        {
            if (SwimBehaviour)
            {
                Transform playerTransform = Player.main.transform;
                SwimBehaviour.LookAt(playerTransform);
                transform.LookAt(playerTransform);
                SwimBehaviour.SwimTo(playerTransform.position, swimSpeed);
            }
            yield return new WaitForSeconds(teleportDelay);
            if (!_isAggressive) break;
            yield return TeleportsBehindYou();
            Player.main.liveMixin.TakeDamage(attackDamage, type: DamageType.Pressure, dealer: gameObject);
            yield return new WaitForSeconds(attackCooldown);
        }
    }

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        damageInfo.damage = 0; // no :^)
        if (damageInfo.dealer != Player.main.gameObject) return;
        SetMode(true);
    }
}
