using Nautilus.Utility;
using SCHIZO.Creatures.Components;
using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark;

partial class ErmsharkAttack
{
    private void Awake()
    {
        attackSounds = attackSounds.Initialize(AudioUtils.BusPaths.UnderwaterCreatures);
    }

    public override void OnTouch(Collider collider)
    {
        if (!enabled) return;
        if (!liveMixin.IsAlive()) return;

        GameObject target = GetTarget(collider);

        if (global::CreatureData.GetCreatureType(gameObject) == global::CreatureData.GetCreatureType(target)) return;
        if (target.GetComponent<GetCarried>()) return; // prevents tutel scream when released

        Player player = target.GetComponent<Player>();
        if (player)
        {
            GameObject heldObject = Inventory.main.GetHeldObject();
            if (heldObject)
            {
                if (heldObject.GetComponent<GetCarried>() is { } heldTutel)
                {
                    Inventory.main.DropHeldItem(false);
                    creature.GetComponent<CarryCreature>().TryPickup(heldTutel);
                    creature.SetFriend(player.gameObject, 120f);
                    return;
                }
                else if (canBeFed && player.CanBeAttacked() && TryEat(heldObject, true))
                {
                    // if (attackSound) Utils.PlayEnvSound(attackSound, mouth.transform.position);
                    gameObject.SendMessage("OnMeleeAttack", heldObject, SendMessageOptions.DontRequireReceiver);
                    return;
                }
            }
        }

        // TODO: verify if ermshark can attack vehicles and cyclopses
        if (CanBite(target))
        {
            timeLastBite = Time.time;
            LiveMixin living = target.GetComponent<LiveMixin>();
            if (living && living.IsAlive())
            {
                living.TakeDamage(GetBiteDamage(target), default, DamageType.Normal, gameObject);
                living.NotifyCreatureDeathsOfCreatureAttack();
            }
            Vector3 damageFxPos = collider.ClosestPointOnBounds(mouth.transform.position);
            if (damageFX != null)
            {
                Instantiate(damageFX, damageFxPos, damageFX.transform.rotation);
            }
            /*if (attackSound != null)
            {
                Utils.PlayEnvSound(attackSound, damageFxPos);
            }*/

            if (attackSounds) attackSounds.Play(emitter.ToFMODEmitter());

            creature.Aggression.Add(-biteAggressionDecrement);
            if (living && !living.IsAlive())
            {
                TryEat(living.gameObject);
            }
            gameObject.SendMessage("OnMeleeAttack", target, SendMessageOptions.DontRequireReceiver);
        }
    }
}
