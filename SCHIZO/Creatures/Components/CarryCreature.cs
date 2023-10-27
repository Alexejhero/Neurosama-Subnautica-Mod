using UnityEngine;

namespace SCHIZO.Creatures.Components;

/// <summary>Adapted from <see cref="CollectShiny"/></summary>
public sealed partial class CarryCreature : CustomCreatureAction
{
    private GetCarried target;
    private bool targetPickedUp;
    private float timeNextFindTarget;
    private float timeNextUpdate;
    private static readonly EcoRegion.TargetFilter _isTargetValidFilter = (IEcoTarget target) => target.GetGameObject().GetComponent<GetCarried>();

    public EcoTargetType EcoTargetType => (EcoTargetType) _ecoTargetType;

    public override void Awake()
    {
        base.Awake();
#warning https://github.com/users/Alexejhero/projects/5/views/1?pane=issue&itemId=42850746
        if (!creature) creature = GetComponent<Creature>();
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

        swimBehaviour.SwimTo(transform.position + Vector3.up + 5f * Random.onUnitSphere, Vector3.up, swimVelocity);
        timeNextUpdate = Time.time + 1f;

        return true;
    }

    private void RepositionTarget(GetCarried getCarried)
    {
        Transform targetTransform = getCarried.transform;
        if (resetRotation) targetTransform.localRotation = Quaternion.identity;

        // place the transform so the pickup point is on the attach point
        Vector3 offset = getCarried.pickupPoint
            ? -getCarried.pickupPoint.localPosition
            : Vector3.zero;
        offset.Scale(attachPoint.lossyScale);
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

    private void DropTarget(GameObject target)
    {
        target.transform.SetParent(null, true);
        UWE.Utils.SetCollidersEnabled(target, true);
        UWE.Utils.SetIsKinematic(target.GetComponent<Rigidbody>(), false);
        if (target.GetComponent<LargeWorldEntity>() is { } lwe)
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
                    LOGGER.LogWarning($"dropping because {roll}<{ADHD}");
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
