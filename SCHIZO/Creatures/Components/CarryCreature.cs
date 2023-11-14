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
        creature.SetFriend(player.gameObject, 120f);
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

        if (!targetPickedUp)
        {
            Vector3 toTarget = target.transform.position - attachSocket.transform.position;
            if (toTarget.sqrMagnitude < pickupRadiusSquared)
            {
                bool pickedUp = targetPickedUp = TryPickup(target);
                if (!pickedUp)
                {
                    target = null;
                    swimToTarget.target = null;
                    timeNextFindTarget = time + updateTargetInterval * 2;
                }
            }
        }
        else
        {
            float roll = Random.value;
            if (roll < ADHD)
            {
                //LOGGER.LogWarning($"dropping because {roll}<{ADHD}");
                Drop();
                return;
            }
            //RepositionTarget(target);

            creature.Happy.Add(deltaTime);
            creature.Friendliness.Add(deltaTime);
        }
    }

    private void UpdateTarget()
    {
        if (targetPickedUp) return;

        IEcoTarget ecoTarget = EcoRegionManager.main!?.FindNearestTarget(EcoTargetType, transform.position, _isTargetValidFilter, 1);
        Carryable newTarget = ecoTarget?.GetGameObject()!?.GetComponent<Carryable>();
        if (!newTarget || newTarget == target || !newTarget.GetComponent<Rigidbody>()) return;

        Vector3 toNewTarget = newTarget.transform.position - transform.position;
        if (Physics.Raycast(transform.position, toNewTarget, toNewTarget.magnitude, Voxeland.GetTerrainLayerMask()))
            return;
        target = newTarget;
        swimToTarget.target = newTarget.transform;
    }

    public bool TryPickup(Carryable getCarried)
    {
        if (!getCarried) return false;
        if (target) Drop();
        target = getCarried;
        return TryPickupTarget();
    }

    private bool TryPickupTarget()
    {
        if (!target || !target.gameObject || !target.gameObject.activeInHierarchy) return false;
        if (!target.CanBePickedUp()) return false;

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
        Quaternion savedRotation = targetTransform.localRotation;
        ModelPlug.PlugIntoSocket(targetTransform, target.attachPlug, attachSocket);
        if (!resetRotation) targetTransform.localRotation = savedRotation;
        target.OnPickedUp(this);
        targetPickedUp = true;

        return true;
    }

    private void RepositionTarget(Carryable getCarried)
    {
        ModelPlug.PlugIntoSocket(getCarried.transform, getCarried.attachPlug, attachSocket);
    }

    private void Drop()
    {
        if (target && targetPickedUp)
        {
            DropTarget(target.gameObject);
            target.OnDropped(this);
        }
        target = null;
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
