using Nautilus.Extensions;
using UnityEngine;

namespace SCHIZO.Items.FumoItem;

partial class EvilFumoItemTool
{
    public Knife stolenKnife;
    private static float _knifeScale = 0.9f;

    private static float _dmgResetTime = 60f;
    private float _timeUntilDamageReset;
    private float _currentDamage;

    private void Start()
    {
        _currentDamage = damageOnPoke;
    }
    protected override void ApplyAltEffect(bool active)
    {
        if (active)
        {
            float dmg = _currentDamage;
            if (stealKnife && TryFindKnife(out Knife knife)
                && Inventory.main.InternalDropItem(knife.pickupable))
            {
                stolenKnife = knife;
                YoinkKnife();
                dmg *= 4;
            }
            usingPlayer.liveMixin.TakeDamage(dmg);
            _currentDamage *= 1.25f; // negative reward function
        }
        else
        {
            if (!ReturnKnife())
                LOGGER.LogError("Could not return stolen knife");
        }
    }

    private bool TryFindKnife(out Knife knife)
    {
        knife = default;

        QuickSlots slots = Inventory.main.quickSlots;
        for (int i = 0; i < 5; i++)
        {
            InventoryItem item = slots.GetSlotItem(i);
            if (item is null || !item.item)
                continue;
            knife = item.item.GetComponent<Knife>();
            if (!knife) continue;
            return true;
        }
        return false;
    }

    private void YoinkKnife()
    {
        UWE.Utils.SetCollidersEnabled(stolenKnife.gameObject, false);
        UWE.Utils.SetIsKinematic(stolenKnife.GetComponent<Rigidbody>(), true);
        UWE.Utils.SetEnabled(stolenKnife.GetComponent<LargeWorldEntity>(), false);
        stolenKnife.transform.SetParent(knifeSocket.Exists() ?? transform, true);
        stolenKnife.transform.localScale *= _knifeScale;
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateDamageReset();
    }

    private void UpdateDamageReset()
    {
        if (_currentDamage == damageOnPoke) return;

        if (isAltEffectActive)
            _timeUntilDamageReset = _dmgResetTime;
        else
        {
            _timeUntilDamageReset -= Time.fixedDeltaTime;
            if (_timeUntilDamageReset < 0)
                _currentDamage = damageOnPoke;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (stolenKnife)
            RepositionKnife(stolenKnife.transform);
    }

    // i don't know what's rotating things *several frames* after they're unparented but i wish it a very pleasant go commit refactor (in copilot)
    private static void RepositionKnife(Transform knife)
    {
        knife.localRotation = Quaternion.identity;
        knife.localPosition = Vector3.zero;
    }

    private bool ReturnKnife()
    {
        if (!stolenKnife) return true;
        DropKnife();

        if (!Inventory.main.Pickup(stolenKnife.pickupable)) return false;
        return true;
    }

    private void DropKnife()
    {
        stolenKnife.transform.localScale /= _knifeScale;
        stolenKnife.transform.SetParent(null, true);
        GameObject colliderTarget = stolenKnife.gameObject;
        FPModel fpModel = stolenKnife.GetComponent<FPModel>();
        if (fpModel) colliderTarget = fpModel.propModel;
        UWE.Utils.SetCollidersEnabled(colliderTarget, true);
        UWE.Utils.SetIsKinematic(stolenKnife.GetComponent<Rigidbody>(), false);
        if (stolenKnife.GetComponent<LargeWorldEntity>().Exists() is { } lwe)
            LargeWorldStreamer.main!?.cellManager.RegisterEntity(lwe);
    }
}
