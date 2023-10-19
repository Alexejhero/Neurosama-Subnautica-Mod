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

    public void TryPickup(GetCarried getCarried = null)
    {
        getCarried ??= target!?.GetComponent<GetCarried>();
        if (getCarried && getCarried.gameObject && getCarried.gameObject.activeInHierarchy)
        {
            if (getCarried.GetComponentInParent<Player>())
            {
                // in player's inventory
                Drop();
                timeNextFindTarget = Time.time + 6f;
                return;
            }
            UWE.Utils.SetCollidersEnabled(getCarried.gameObject, false);
            getCarried.transform.parent = attachPoint;
            getCarried.transform.localPosition = Vector3.zero;
            getCarried.OnPickedUp();
            getCarried.swimBehaviour.Idle();
            targetPickedUp = true;
            UWE.Utils.SetIsKinematic(getCarried.GetComponent<Rigidbody>(), true);
            UWE.Utils.SetEnabled(getCarried.GetComponent<LargeWorldEntity>(), false);
            swimBehaviour.SwimTo(transform.position + Vector3.up + 5f * Random.onUnitSphere, Vector3.up, swimVelocity);
            timeNextUpdate = Time.time + 1f;
        }
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
        target.transform.parent = null;
        UWE.Utils.SetCollidersEnabled(target, true);
        UWE.Utils.SetIsKinematic(target.GetComponent<Rigidbody>(), false);
        if (target.GetComponent<LargeWorldEntity>() is { } lwe)
            LargeWorldStreamer.main!?.cellManager.RegisterEntity(lwe);
    }

    private void UpdateTarget()
    {
        IEcoTarget ecoTarget = EcoRegionManager.main!?.FindNearestTarget(EcoTargetType.Coral, transform.position, _isTargetValidFilter, 1);
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
                TryPickup();
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
                    return;
                }
                else
                {
                    TryPickup();
                }
            }
            if (time > timeNextUpdate)
            {
                timeNextUpdate = time + updateInterval;
                swimBehaviour.SwimTo(transform.position + 2f * swimVelocity * Random.insideUnitSphere, swimVelocity);
                if (Random.value < ADHD) Drop();
            }
            creature.Happy.Add(deltaTime);
            creature.Friendliness.Add(deltaTime);
        }
    }

    public override void StopPerform(float time) => Drop();

    private void OnDisable() => Drop();
}
