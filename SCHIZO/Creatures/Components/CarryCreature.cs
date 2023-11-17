using System.Collections;
using UnityEngine;

namespace SCHIZO.Creatures.Components;

partial class CarryCreature : IOnTakeDamage, IOnMeleeAttack
{
    public Carryable target;
    public Creature creature;
    public bool targetPickedUp;

    private float pickupRadiusSquared;
    private float timeNextFindTarget;
    private float timeNextUpdate;
    private static readonly EcoRegion.TargetFilter _isTargetValidFilter = target => target.GetGameObject().GetComponent<Carryable>();

    public EcoTargetType EcoTargetType => (EcoTargetType) _ecoTargetType;

    public void Awake()
    {
        creature = GetComponent<Creature>();
        pickupRadiusSquared = Mathf.Pow(attachRadius, 2f);
        Pickupable pickupable = GetComponent<Pickupable>();
        if (pickupable) pickupable.pickedUpEvent.AddHandler(gameObject, (_) => Drop());
    }

    public void OnDestroy()
    {
        Pickupable pickupable = GetComponent<Pickupable>();
        if (pickupable) pickupable.pickedUpEvent.RemoveHandlers(gameObject);
    }

    void IOnTakeDamage.OnTakeDamage(DamageInfo damageInfo)
    {
        if (damageInfo.damage > 0) Drop();
    }

    bool IOnMeleeAttack.HandleMeleeAttack(GameObject targetObject)
    {
        // prevent bite after releasing
        if (targetObject.GetComponent<Carryable>()) return true;

        // pick up held creature instead of eating it
        Player player = targetObject.GetComponent<Player>();
        if (!player) return false;

        GameObject heldObject = Inventory.main.GetHeldObject();
        if (!heldObject) return false;

        Carryable heldCarryable = heldObject.GetComponent<Carryable>();
        if (!heldCarryable) return false;

        EcoTargetType filterType = EcoTargetType;
        EcoTargetType targetType = heldObject.GetComponent<IEcoTarget>()?.GetTargetType()
            ?? EcoTargetType.None;
        if (filterType != targetType) return false;

        Inventory.main.DropHeldItem(false);
        heldObject.SetActive(true); // really makes you think
        StartCoroutine(DelayedPickupHack());
        if (creature) creature.SetFriend(player.gameObject, 120f);
        return true;

        IEnumerator DelayedPickupHack() // not sure how or why but WM sometimes doesn't get scaled correctly unless we wait a frame
        {
            yield return null;
            TryPickup(heldCarryable);
        }
    }

    public void FixedUpdate()
    {
        float time = Time.fixedTime;
        float deltaTime = Time.fixedDeltaTime;
        if (time > timeNextFindTarget)
        {
            UpdateTarget();
            timeNextFindTarget = time + updateTargetInterval * (1f + 0.2f * Random.value);
        }

        if (!target) return;

        if (time < timeNextUpdate) return;
        timeNextUpdate = time + updateInterval;

        if (!targetPickedUp)
        {
            swimToTarget.target = target.transform;
            Vector3 toTarget = target.transform.position - attachSocket.transform.position;
            if (toTarget.sqrMagnitude < pickupRadiusSquared)
            {
                bool pickedUp = targetPickedUp = TryPickup(target);
                if (!pickedUp)
                {
                    ClearTarget();
                    timeNextFindTarget = time + updateTargetInterval * 2;
                }
            }
        }
        else
        {
            RepositionTarget();
            if (ADHD > 0)
            {
                float roll = Random.value;
                if (roll < ADHD)
                {
                    //LOGGER.LogWarning($"dropping because {roll}<{ADHD}");
                    Drop();
                    return;
                }
            }
            if (creature)
            {
                creature.Happy.Add(deltaTime);
                creature.Friendliness.Add(deltaTime);
            }
        }
    }

    private void UpdateTarget()
    {
        if (targetPickedUp || EcoTargetType == EcoTargetType.None) return;

        IEcoTarget ecoTarget = EcoRegionManager.main!?.FindNearestTarget(EcoTargetType, transform.position, _isTargetValidFilter, 1);
        Carryable newTarget = ecoTarget?.GetGameObject()!?.GetComponent<Carryable>();
        if (!newTarget || newTarget == target || !newTarget.GetComponent<Rigidbody>()) return;
        if (newTarget.gameObject == gameObject) return; // holy hell

        Vector3 toNewTarget = newTarget.transform.position - transform.position;
        if (Physics.Raycast(transform.position, toNewTarget, toNewTarget.magnitude, Voxeland.GetTerrainLayerMask()))
            return;
        SetTarget(newTarget);
    }

    public void SetTarget(Carryable newTarget)
    {
        if (!newTarget) return;
        if (newTarget.gameObject == gameObject) return;

        target = newTarget;
        Transform targetTransform = newTarget.attachPlug !?? newTarget.transform;
        swimToTarget.target = targetTransform;
    }

    public void ClearTarget()
    {
        target = null;
        swimToTarget.target = null;
    }

    public bool TryPickup(Carryable getCarried)
    {
        if (!getCarried || getCarried.gameObject == gameObject) return false;
        if (target) Drop();
        target = getCarried;
        return TryPickupTarget();
    }

    private bool TryPickupTarget()
    {
        if (!target || !target.gameObject || !target.gameObject.activeInHierarchy) return false;
        if (!target.CanBePickedUp(this)) return false;

        if (target.GetComponentInParent<Player>())
        {
            // in player's inventory
            Drop();
            return false;
        }
        UWE.Utils.SetCollidersEnabled(target.gameObject, false);
        UWE.Utils.SetIsKinematic(target.GetComponent<Rigidbody>(), true);
        UWE.Utils.SetEnabled(target.GetComponent<LargeWorldEntity>(), false);

        Transform targetTransform = target.transform;
        targetTransform.SetParent(attachSocket, true); // false sets scale incorrectly

        RepositionTarget();
        StartCoroutine(DelayedReposition()); // some component just really likes running updates for a few frames after it gets disabled

        target.OnPickedUp(this);
        targetPickedUp = true;
        swimToTarget.target = null;

        return true;

        IEnumerator DelayedReposition()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSeconds(0.05f);
                RepositionTarget();
            }
        }
    }

    private void RepositionTarget()
    {
        Transform targetTransform = target.transform;
        if (resetRotation) targetTransform.localRotation = Quaternion.identity;

        targetTransform.localPosition = Vector3.zero;
        // place the transform so the plug is exactly on the socket
        Vector3 offset = Vector3.zero;

        if (target.attachPlug)
        {
            offset = attachSocket.InverseTransformPoint(target.attachPlug.position);
        }

        targetTransform.localPosition = -offset;
    }

    public void Drop()
    {
        if (target && targetPickedUp)
        {
            DropTarget(target.gameObject);
            target.OnDropped(this);
        }
        SetTarget(null);
        targetPickedUp = false;
    }

    private void DropTarget(GameObject targetObject)
    {
        targetObject.transform.SetParent(null, true);
        GameObject colliderTarget = targetObject;
        FPModel fpModel = targetObject.GetComponent<FPModel>();
        if (fpModel) colliderTarget = fpModel.propModel;
        UWE.Utils.SetCollidersEnabled(colliderTarget, true);
        UWE.Utils.SetIsKinematic(targetObject.GetComponent<Rigidbody>(), false);
        if (targetObject.GetComponent<LargeWorldEntity>() is { } lwe)
            LargeWorldStreamer.main!?.cellManager.RegisterEntity(lwe);
    }

    public void OnDisable() => Drop();
}
