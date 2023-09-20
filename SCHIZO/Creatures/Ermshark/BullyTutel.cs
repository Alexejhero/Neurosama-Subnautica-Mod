using ProtoBuf;
using SCHIZO.Creatures.Tutel;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark;

/// <summary>Adapted from <see cref="CollectShiny"/></summary>
[ProtoContract]
[RequireComponent(typeof(SwimBehaviour))]
public class BullyTutel : CreatureAction, IProtoTreeEventListener
{
    private void Awake()
    {
        isTargetValidFilter = IsTargetValid;
    }

#if BELOWZERO
    public override float Evaluate(float time) => EvaluateCore(creature, time);
    public override void Perform(float time, float deltaTime) => PerformCore(creature, time, deltaTime);
    public override void StopPerform(float time) => StopPerformCore(creature, time);
#else
    public override float Evaluate(Creature creat, float time) => EvaluateCore(creat, time);
    public override void Perform(Creature creat, float time, float deltaTime) => PerformCore(creat, time, deltaTime);
    public override void StopPerform(Creature creat, float time) => StopPerformCore(creat, time);
#endif
    public float EvaluateCore(Creature creat, float time)
    {
        if (timeNextFindTutel < time)
        {
            UpdateBullyTarget();
            if (tutel)
            {
                creat.Aggression.Value = 0;
                creat.Curiosity.Value = 10;
            }
            timeNextFindTutel = time + updateTargetInterval * (1f + 0.2f * Random.value);
        }
        return tutel && tutel.gameObject.activeInHierarchy ? GetEvaluatePriority() : 0f;
    }

    public void StopPerformCore(Creature creat, float time)
    {
        DropTutel();
    }

    public void TryPickupTutel(GetCarried getCarried = null)
    {
        getCarried ??= tutel!?.GetComponent<GetCarried>();
        if (getCarried && getCarried.gameObject && getCarried.gameObject.activeInHierarchy)
        {
            if (getCarried.GetComponentInParent<Player>())
            {
                // in player's inventory
                DropTutel();
                timeNextFindTutel = Time.time + 6f;
                return;
            }
            UWE.Utils.SetCollidersEnabled(getCarried.gameObject, false);
            getCarried.transform.parent = tutelAttach;
            getCarried.transform.localPosition = Vector3.zero;
            getCarried.OnPickedUp();
            getCarried.GetComponent<SwimBehaviour>().Idle();
            targetPickedUp = true;
            UWE.Utils.SetIsKinematic(getCarried.GetComponent<Rigidbody>(), true);
            UWE.Utils.SetEnabled(getCarried.GetComponent<LargeWorldEntity>(), false);
            swimBehaviour.SwimTo(transform.position + Vector3.up + 5f * Random.onUnitSphere, Vector3.up, swimVelocity);
            timeNextUpdate = Time.time + 1f;
        }
    }

    private void DropTutel()
    {
        if (tutel && targetPickedUp)
        {
            DropTutelTarget(tutel.gameObject);
            tutel.GetComponent<GetCarried>()?.OnDropped();
        }
        tutel = null;
        targetPickedUp = false;
    }

    private void DropTutelTarget(GameObject target)
    {
        target.transform.parent = null;
        UWE.Utils.SetCollidersEnabled(target, true);
        UWE.Utils.SetIsKinematic(target.GetComponent<Rigidbody>(), false);
        if (target.GetComponent<LargeWorldEntity>() is { } lwe)
            LargeWorldStreamer.main!?.cellManager.RegisterEntity(lwe);
    }

    private static bool IsTargetValid(IEcoTarget target) => TutelLoader.TutelTechTypes.Contains(CraftData.GetTechType(target.GetGameObject()));

    private void UpdateBullyTarget()
    {
        IEcoTarget ecoTarget = EcoRegionManager.main?.FindNearestTarget(EcoTargetType.Coral, transform.position, isTargetValidFilter, 1);
        Creature newTarget = ecoTarget?.GetGameObject().GetComponent<Creature>();
        if (!newTarget) return;

        Vector3 direction = newTarget.transform.position - transform.position;
        float dist = direction.magnitude - 0.5f;
        if (dist > 0 && Physics.Raycast(transform.position, direction, dist, Voxeland.GetTerrainLayerMask()))
            return;
        if (tutel == newTarget || !newTarget.GetComponent<Rigidbody>() || !newTarget.GetComponent<Pickupable>())
            return;
        if (tutel)
        {
            float sqrDistToNew = direction.sqrMagnitude;
            float sqrDistToCurr = (tutel.transform.position - transform.position).sqrMagnitude;
            if (sqrDistToNew > sqrDistToCurr)
            {
                DropTutel();
            }
        }
        tutel = newTarget;
    }

    public void PerformCore(Creature creat, float time, float deltaTime)
    {
        if (!tutel) return;
        if (!targetPickedUp)
        {
            if (time > timeNextUpdate)
            {
                timeNextUpdate = time + updateInterval;
                swimBehaviour.SwimTo(tutel.transform.position, -Vector3.up, swimVelocity);
            }
            if ((transform.position - tutel.transform.position).sqrMagnitude < 9f)
            {
                TryPickupTutel();
                return;
            }
        }
        else
        {
            if (tutel.transform.parent != tutelAttach)
            {
                if (tutel.transform.parent && tutel.transform.parent.GetComponentInParent<Creature>())
                {
                    // someone else snatched him
                    targetPickedUp = false;
                    tutel = null;
                }
                else
                {
                    TryPickupTutel();
                }
            }
            if (time > timeNextUpdate)
            {
                timeNextUpdate = time + updateInterval;
                swimBehaviour.SwimTo(transform.position + 2f * swimVelocity * Random.insideUnitSphere, swimVelocity);
                if (Random.value < 0.15f) DropTutel();
            }
            creat.Happy.Add(deltaTime);
            creat.Friendliness.Add(deltaTime);
        }
    }

    private void OnDisable()
    {
        DropTutel();
    }


    public void OnProtoSerializeObjectTree(ProtobufSerializer serializer)
    {
    }

    public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
    {
        foreach (object obj in tutelAttach)
        {
            Transform trans = (Transform) obj;
            DropTutelTarget(trans.gameObject);
        }
    }

    [AssertNotNull]
    public Transform mouth;
    [AssertNotNull]
    public Transform tutelAttach;
    private Creature tutel;
    private bool targetPickedUp;
    private float timeNextFindTutel;
    public float swimVelocity = ErmsharkPrefab.swimVelocity;
    public float updateInterval = 2f;
    public float updateTargetInterval = 1f;
    private float timeNextUpdate;
    private EcoRegion.TargetFilter isTargetValidFilter;
}
