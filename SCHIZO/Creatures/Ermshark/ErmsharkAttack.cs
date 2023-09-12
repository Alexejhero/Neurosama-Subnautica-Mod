using SCHIZO.Creatures.Tutel;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark;

public sealed class ErmsharkAttack : MeleeAttack
{
    public override void OnTouch(Collider collider)
    {
        if (!enabled) return;
        if (!liveMixin.IsAlive()) return;

        GameObject target = GetTarget(collider);

        if (CreatureData.GetCreatureType(gameObject) == CreatureData.GetCreatureType(target)) return;

        Player component = target.GetComponent<Player>();
        if (component && canBeFed && component.CanBeAttacked())
        {
            GameObject heldObject = Inventory.main.GetHeldObject();
            if (heldObject)
            {
                if (TryEat(heldObject, true))
                {
                    if (attackSound)
                    {
                        Utils.PlayEnvSound(attackSound, mouth.transform.position);
                    }
                    gameObject.SendMessage("OnMeleeAttack", heldObject, SendMessageOptions.DontRequireReceiver);
                    return;
                }
                if (heldObject.GetComponent<GetCarried>() is { } tutel)
                {
                    Inventory.main.DropHeldItem(false);
                    creature.GetComponent<BullyTutel>().TryPickupTutel(tutel);
                    creature.SetFriend(component.gameObject, 120f);
                    return;
                }
            }
        }

        if (CanBite(target))
        {
            timeLastBite = Time.time;
            LiveMixin component2 = target.GetComponent<LiveMixin>();
            if (component2 != null && component2.IsAlive())
            {
                component2.TakeDamage(GetBiteDamage(target), default, DamageType.Normal, gameObject);
                component2.NotifyCreatureDeathsOfCreatureAttack();
            }
            Vector3 vector = collider.ClosestPointOnBounds(mouth.transform.position);
            if (damageFX != null)
            {
                Instantiate(damageFX, vector, damageFX.transform.rotation);
            }
            if (attackSound != null)
            {
                Utils.PlayEnvSound(attackSound, vector);
            }

            ErmsharkLoader.AttackSounds.Play(gameObject.GetComponent<FMOD_CustomEmitter>());

            creature.Aggression.Add(-biteAggressionDecrement);
            if (component2 != null && !component2.IsAlive())
            {
                TryEat(component2.gameObject);
            }
            gameObject.SendMessage("OnMeleeAttack", target, SendMessageOptions.DontRequireReceiver);
        }
    }
}
