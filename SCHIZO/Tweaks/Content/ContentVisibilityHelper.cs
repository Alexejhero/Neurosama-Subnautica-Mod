using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SCHIZO.Tweaks.Content;

partial class ContentVisibilityHelper
{
    private float _blinkTimer;
    private List<Renderer> _renderers;
    private bool _dead;

    private void Start()
    {
        ContentAlertManager.OnAlertsEnabledChanged += Changed;
        Changed();
        _renderers = [];
        transform.parent.GetComponentsInChildren(true, _renderers);
        _renderers.RemoveAll(r => r.transform.IsChildOf(transform));

        if (_renderers.Count < 1 || !_renderers[0])
        {
            _dead = true;
            gameObject.SetActive(false);
            return;
        }

        // scale self with parent's mesh bounds
        Vector3 origScale = transform.localScale;
        Bounds bounds = _renderers[0].bounds;
        foreach (Renderer r in _renderers.Where(r => r.enabled))
            bounds.Encapsulate(r.bounds);
        Bounds ourBounds = new(transform.position, Vector3.zero);
        foreach (Renderer r in GetComponentsInChildren<Renderer>(true))
            ourBounds.Encapsulate(r.bounds);
        float size = (bounds.size.x + bounds.size.y + bounds.size.z) / 3;
        float ourSize = (ourBounds.size.x + ourBounds.size.y + ourBounds.size.z) / 3;
        float scale = Mathf.Max(0.5f, size / ourSize);
        transform.localScale = scale * origScale;

        Pickupable pickupable = transform.parent.gameObject.GetComponent<Pickupable>();
        if (pickupable)
        {
            pickupable.pickedUpEvent.AddHandler(this, (_) => enabled = false);
            pickupable.droppedEvent.AddHandler(this, (_) => enabled = true);
        }
    }

    private void OnDestroy()
    {
        ContentAlertManager.OnAlertsEnabledChanged -= Changed;
        Pickupable pickupable = transform.parent.gameObject.GetComponent<Pickupable>();
        if (pickupable)
        {
            pickupable.pickedUpEvent.RemoveHandler(this);
            pickupable.droppedEvent.RemoveHandler(this);
        }
    }

    private void OnEnable() => Changed();
    private void OnDisable() => Changed();

    private void Changed()
    {
        _blinkTimer = blinkOnDuration;
        target.gameObject.SetActive(enabled && ContentAlertManager.AlertsEnabled);
    }
    private void LateUpdate()
    {
        if (_dead) return;
        if (!ContentAlertManager.AlertsEnabled) return;

        Vector3 pos = Camera.main.transform.position;
        //pos.y = target.pos;
        target.LookAt(pos);

        _blinkTimer -= Time.deltaTime;
        //           <------- timer --------
        // -blinkOffDuration....0....blinkOnDuration
        // ^--------off--------^^--------on--------^
        if (_blinkTimer <= -blinkOffDuration)
            _blinkTimer = blinkOnDuration;
        target.gameObject.SetActive(_blinkTimer >= 0);
    }
}
