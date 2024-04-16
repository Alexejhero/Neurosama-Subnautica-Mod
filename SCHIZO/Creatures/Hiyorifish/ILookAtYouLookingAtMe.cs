using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using RuntimeDebugDraw;
using SCHIZO.Helpers;
using UnityEngine;
using UWEXR;

namespace SCHIZO.Creatures.Hiyorifish;

partial class ILookAtYouLookingAtMe
{
    private bool _isObscured;

    private RaycastHit[] _hits = [];
    [UsedImplicitly] // do not remove, do not make const or readonly, do not do anything
    public bool debug = false;

    private void FixedUpdate()
    {
        if (!Player.main) return;

        Vector3 camPos = MainCameraControl.main.transform.position;
        Vector3 lineOfSight = transform.position - camPos;
        int max = UWE.Utils.RaycastIntoSharedBuffer(camPos,
            lineOfSight.normalized, lineOfSight.magnitude,
            layerMask: -1, queryTriggerInteraction: QueryTriggerInteraction.Ignore);
        IEnumerable<RaycastHit> obscuringHits = UWE.Utils.sharedHitBuffer.Take(max)
            .Where(hit =>
            {
                GameObject hitGO = hit.collider.attachedRigidbody!?.gameObject
                    ?? hit.collider.gameObject;
                return hitGO != gameObject && hitGO != Player.main.gameObject;
            });
        _isObscured = obscuringHits.Any();
        if (debug && !Input.GetKey(KeyCode.LeftAlt))
        {
            _hits = obscuringHits.ToArray();
            _hits.ForEach(hit => {
                Draw.DrawText(hit.point, hit.collider.gameObject.name, Color.yellow, 16, Time.fixedDeltaTime, false);
                Draw.DrawCube(hit.collider.bounds.min, hit.collider.bounds.max, Color.yellow, duration: Time.fixedDeltaTime);
                Draw.DrawRay(hit.point, hit.normal, Color.blue, duration: Time.fixedDeltaTime);
            });
        }
    }

    private void Update()
    {
        if (debug && !Input.GetKey(KeyCode.LeftAlt))
        {
            Draw.DrawLine(MainCameraControl.main.transform.position, transform.position, _isObscured ? Color.red : Color.green);
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
        if (!enabled || !Player.main || _isObscured) return;
        // vomit now
        if (XRSettings.enabled) return;

        MainCameraControl cam = MainCameraControl.main;
        Vector3 plrToMe = transform.position - cam.transform.position;
        float distance = plrToMe.magnitude;
        Vector3 direction = plrToMe.normalized;
        if (distance > maxRange) return;

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
        float turnSpeed = (camFov - angle) * distanceScalar * lookTurnPower;
        Vector2 turn = lookError.normalized * turnSpeed * Time.deltaTime;

        if (debug && !Input.GetKey(KeyCode.LeftAlt))
        {
            Vector3 lookWorldPos = MainCamera.camera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, MainCamera.camera.nearClipPlane));
            Draw.DrawLine(transform.position, lookWorldPos, Color.cyan, depthTest: false);
            if (Input.GetKey(KeyCode.RightAlt))
                Debugger.Break();
            return;
        }

        // rotationX/Y is yaw/pitch, as god intended (Vector x/y is pitch/yaw)
        cam.rotationX += turn.y;
        cam.rotationY -= turn.x;
    }

    // todo invert lookTurnPower for attack mode or something

    // todo pop when offscreen
    private void OnBecameInvisible()
    {
        
    }
}
