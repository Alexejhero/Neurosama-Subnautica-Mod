using System.Collections.Generic;
using System.Linq;
using SCHIZO.Creatures.Components;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Creatures.Ermfish;

/// <summary>
/// erm (tail)<br/>
/// ...<br/>
/// erm ^-socket side-^ v-plug side-v<br/>
/// ...<br/>
/// erm (head) &lt;-- has swim control<br/>
/// </summary>
public sealed partial class ErmStack
{
    // TODO: actual stack behaviour (push/pop) just to be funny

    public ErmStack head;
    public ErmStack tail;

    public ErmStack nextPlug;
    public ErmStack nextSocket;

    private float _nextUpdate;

    public void Start()
    {
        head = this;
        tail = this;
        plug.Attached += OnConnected;
        plug.Detached += OnDisconnected;
        plug.CanAttach += ShouldAttach;

        socket.ADHD = 0f; // we will detach manually
    }

    public void FixedUpdate()
    {
        if (Time.fixedTime > _nextUpdate)
        {
            _nextUpdate = Time.fixedTime + updateInterval;

            float roll = Random.Range(0f, 1f);
            // LOGGER.LogWarning($"rolled {roll}");

            if (!nextSocket && !nextPlug)
            {
                if (socket.target) return;
                if (roll < growStackChance) StartStacking();
            }
            else
            {
                float detachChance = nextSocket && nextPlug ? middleDetachChance : endsDetachChance;
                if (roll < detachChance)
                {
                    Disconnect(Random.Range(0f, 1f) < 0.5f);
                }
            }
        }
    }

    public void StartStacking()
    {
        ErmStack target = FindTarget();
        if (!target || target == this) return;

        socket.SetTarget(target.plug);
    }

    public ErmStack FindTarget()
    {
        TechType selfTechType = CraftData.GetTechType(gameObject);
        bool EcoTargetFilter(IEcoTarget et) => CraftData.GetTechType(et.GetGameObject()) == selfTechType && et.GetGameObject() != gameObject;

        ErmStack target = FindNearest_EcoTarget() !?? FindNearest_TechType();
        return target;

        ErmStack FindNearest_EcoTarget()
        {
            EcoTarget selfEcoTarget = GetComponent<EcoTarget>();
            if (!selfEcoTarget || selfEcoTarget.type == EcoTargetType.None) return null;

            return EcoRegionManager.main!?
                .FindNearestTarget(selfEcoTarget.type, transform.position, EcoTargetFilter)?
                .GetGameObject()!?
                .GetComponent<ErmStack>();
        }

        ErmStack FindNearest_TechType()
        {
            return PhysicsHelpers.ObjectsInRange(gameObject.transform.position, 20f)
                .OfTechType(selfTechType)
                .SelectComponentInParent<ErmStack>()
                .OrderByDistanceTo(gameObject.transform.position)
                .FirstOrDefault(s => !s.nextSocket && s != this);
        }
    }

    public static bool Connect(ErmStack plug, ErmStack socket)
    {
        if (plug.nextSocket)
        {
            LOGGER.LogError($"Tried to connect plug '{plug}' to socket '{socket}' but plug is already attached to {plug.nextSocket}!\nDisconnecting!");
            plug.Disconnect(false);
        }
        if (socket.nextPlug)
        {
            LOGGER.LogError($"Tried to connect plug '{plug}' to socket '{socket}' but socket is already attached to {socket.nextPlug}!\nDisconnecting!");
            socket.Disconnect(true);
        }

        return socket.socket.TryPickup(plug.plug);
    }

    /// <summary>
    /// Disconnect a <paramref name="node"/> from the stack.
    /// </summary>
    /// <param name="node">Node to disconnect.</param>
    /// <param name="plugSide">Which side to disconnect from. Only has an effect on middle nodes.</param>
    public static void Disconnect(ErmStack node, bool plugSide = true)
    {
        if (!node.nextSocket && !node.nextPlug) return;

        // Disconnect only actually does anything on the socket side
        CarryCreature socket;
        if (node.nextSocket && node.nextPlug)
        {
            // both connected - respect plugSide
            socket = plugSide ? node.nextSocket.socket : node.socket;
        }
        else
        {
            socket = node.nextSocket ? node.nextSocket.socket : node.socket;
        }
        socket.Drop();
    }

    public bool Connect(ErmStack node, bool nodeIsPlug) => nodeIsPlug ? Connect(node, this) : Connect(this, node);
    public void Disconnect(bool plugSide = true) => Disconnect(this, plugSide);

    public bool ShouldAttach(Carryable _, CarryCreature socket)
    {
        ErmStack socketStack = socket.GetComponent<ErmStack>();
        if (!socketStack) return true;

        return socketStack.head != head && socketStack.tail != tail
            && !socketStack.nextPlug;
    }

    public void OnConnected(Carryable plug, CarryCreature socket)
    {
        // only called on the plug side - so we update the socket from the plug
        ErmStack socketStack = socket.GetComponent<ErmStack>();
        if (!socketStack) return;

        socketStack.nextPlug = this;
        nextSocket = socketStack;

        WalkToTail().ForEach(s => s.head = socketStack.head);
        socketStack.WalkToHead().ForEach(s => s.tail = tail);
    }

    public void OnDisconnected(Carryable plug, CarryCreature socket)
    {
        // only called on the plug side - so we update the socket from the plug
        ErmStack socketStack = socket.GetComponent<ErmStack>();
        if (!socketStack) return;

        socketStack.nextPlug = null;
        nextSocket = null;

        WalkToTail().ForEach(s => s.head = this);
        socketStack.WalkToHead().ForEach(s => s.tail = socketStack);
    }

    private IEnumerable<ErmStack> WalkToHead()
    {
        yield return this;
        for (ErmStack curr = nextSocket; curr; curr = curr.nextSocket)
        {
            if (curr == this)
            {
                LOGGER.LogError($"Cycle in {nameof(WalkToHead)} - {nameof(nextSocket)} is self\n{StackTraceUtility.ExtractStackTrace()}");
                yield break;
            }
            yield return curr;
        }
    }

    private IEnumerable<ErmStack> WalkToTail()
    {
        yield return this;
        for (ErmStack curr = nextPlug; curr; curr = curr.nextPlug)
        {
            if (curr == this)
            {
                LOGGER.LogError($"Cycle in {nameof(WalkToTail)} - {nameof(nextPlug)} is self\n{StackTraceUtility.ExtractStackTrace()}");
                yield break;
            }
            yield return curr;
        }
    }
}
