using System.Collections;
using UnityEngine;

namespace SCHIZO.Creatures.Components;

/// <summary>Adapted from <see cref="CollectShiny"/></summary>
partial class CarryCreature : IOnTakeDamage
{
    private GetCarried target;
    private bool targetPickedUp;
    private float timeNextFindTarget;
    private float timeNextUpdate;
    private static readonly EcoRegion.TargetFilter _isTargetValidFilter = target => target.GetGameObject().GetComponent<GetCarried>();

    public EcoTargetType EcoTargetType => (EcoTargetType) _ecoTargetType;

    void IOnTakeDamage.OnTakeDamage(DamageInfo damageInfo)
    {
        if (damageInfo.damage > 0) Drop();
    }

    bool IOnMeleeAttack.HandleMeleeAttack(GameObject target)
    {
        // prevent bite after releasing
        if (target.GetComponent<GetCarried>()) return true;

        // pick up held creature instead of eating it
        Player player = target.GetComponent<Player>();
        if (!player) return false;

        GameObject heldObject = Inventory.main.GetHeldObject();
        if (!heldObject) return false;

        GetCarried heldCarryable = heldObject.GetComponent<GetCarried>();
        if (!heldCarryable) return false;

        if (EcoTargetType != heldObject.GetComponent<IEcoTarget>()?.GetTargetType()) return false;

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

    public override float Evaluate(float time)
    {
        if (timeNextFindTarget < time)
        {
            UpdateTarget();
            if (target)
            {
                creature.Aggression.Value = 0;
                creature.Curiosity.Value = 10;
            }
            timeNextFindTarget = time + updateTargetInterval * (1f + 0.2f * Random.value);
        }
        return target && target.gameObject.activeInHierarchy ? GetEvaluatePriority() : 0f;
    }

    public bool TryPickup(GetCarried getCarried)
    {
        if (!getCarried) return false;
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
            timeNextFindTarget = Time.time + 6f;
            return false;
        }
        UWE.Utils.SetCollidersEnabled(target.gameObject, false);
        UWE.Utils.SetIsKinematic(target.GetComponent<Rigidbody>(), true);
        UWE.Utils.SetEnabled(target.GetComponent<LargeWorldEntity>(), false);

        Transform targetTransform = target.transform;
        targetTransform.SetParent(attachPoint, true); // false sets scale incorrectly
        target.OnPickedUp();
        targetPickedUp = true;

        RepositionTarget(target);
        StartCoroutine(DelayedReposition()); // some component just really likes running updates for a few frames after it gets disabled

        swimBehaviour.SwimTo(transform.position + Vector3.up + 5f * Random.onUnitSphere, Vector3.up, swimVelocity);
        timeNextUpdate = Time.time + 1f;

        return true;

        IEnumerator DelayedReposition()
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSeconds(0.05f);
                RepositionTarget(target);
            }
        }
    }

    private void RepositionTarget(GetCarried getCarried)
    {
        Transform targetTransform = getCarried.transform;
        if (resetRotation) targetTransform.localRotation = Quaternion.identity;

        // place the transform so the pickup point is on the attach point
        Vector3 offset = getCarried.pickupPoint
            ? -getCarried.pickupPoint.localPosition
            : Vector3.zero;
        offset.Scale(new Vector3(1f / targetTransform.localScale.x, 1f / targetTransform.localScale.y, 1f / targetTransform.localScale.z));
        targetTransform.localPosition = offset;
    }

    private void Drop()
    {
        if (target && targetPickedUp)
        {
            DropTarget(target.gameObject);
            target.OnDropped();
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

    private void UpdateTarget()
    {
        IEcoTarget ecoTarget = EcoRegionManager.main!?.FindNearestTarget(EcoTargetType, transform.position, _isTargetValidFilter, 1);
        GetCarried newTarget = ecoTarget?.GetGameObject()!?.GetComponent<GetCarried>();
        if (!newTarget) return;

        Vector3 toNewTarget = newTarget.transform.position - transform.position;
        float dist = toNewTarget.magnitude - 0.5f;
        if (dist > 0 && Physics.Raycast(transform.position, toNewTarget, dist, Voxeland.GetTerrainLayerMask()))
            return;
        if (target == newTarget || !newTarget.GetComponent<Rigidbody>())
            return;
        if (target)
        {
            float sqrDistToNew = toNewTarget.sqrMagnitude;
            float sqrDistToCurr = (target.transform.position - transform.position).sqrMagnitude;

            if (sqrDistToNew > sqrDistToCurr)
                Drop();
        }
        target = newTarget;
    }

    public override void Perform(float time, float deltaTime)
    {
        if (!target) return;
        if (!targetPickedUp)
        {
            if (time > timeNextUpdate)
            {
                timeNextUpdate = time + updateInterval;
                swimBehaviour.SwimTo(target.transform.position, -Vector3.up, swimVelocity);
            }
            if ((attachPoint.transform.position - target.transform.position).sqrMagnitude < Mathf.Pow(attachRadius, 2f))
            {
                TryPickupTarget();
                return;
            }
        }
        else
        {
            if (target.transform.parent != attachPoint)
            {
                if (target.transform.parent && target.transform.parent.GetComponentInParent<CarryCreature>())
                {
                    // picked up by someone else
                    Drop();
                }
                else
                {
                    TryPickupTarget();
                }
                return;
            }
            if (time > timeNextUpdate)
            {
                timeNextUpdate = time + updateInterval;
                RepositionTarget(target); // sometimes the target moves (for absolutely no reason) after it gets attached
                swimBehaviour.SwimTo(transform.position + 2f * swimVelocity * Random.insideUnitSphere, swimVelocity);
                float roll = Random.value;
                if (roll < ADHD)
                {
                    //LOGGER.LogWarning($"dropping because {roll}<{ADHD}");
                    Drop();
                }
            }
            creature.Happy.Add(deltaTime);
            creature.Friendliness.Add(deltaTime);
        }
    }

    public override void StopPerform(float time) => Drop();

    private void OnDisable() => Drop();
}
