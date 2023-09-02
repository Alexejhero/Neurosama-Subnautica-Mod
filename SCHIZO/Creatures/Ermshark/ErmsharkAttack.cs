using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark;

public sealed class ErmsharkAttack : MeleeAttack
{
    public override void OnTouch(Collider collider)
    {
        if (!enabled) return;
        GameObject target = GetTarget(collider);

        if (ignoreSameKind && CreatureData.GetCreatureType(gameObject) == CreatureData.GetCreatureType(target)) return;
        if (!liveMixin.IsAlive()) return;

        Player component = target.GetComponent<Player>();
        if (component != null && canBeFed && component.CanBeAttacked())
        {
            GameObject heldObject = Inventory.main.GetHeldObject();
            if (heldObject != null && TryEat(heldObject, true))
            {
                if (attackSound != null)
                {
                    Utils.PlayEnvSound(attackSound, mouth.transform.position);
                }
                gameObject.SendMessage("OnMeleeAttack", heldObject, SendMessageOptions.DontRequireReceiver);
                return;
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

            ErmsharkLoader.AttackSounds.Play();

            creature.Aggression.Add(-biteAggressionDecrement);
            if (component2 != null && !component2.IsAlive())
            {
                TryEat(component2.gameObject);
            }
            gameObject.SendMessage("OnMeleeAttack", target, SendMessageOptions.DontRequireReceiver);
        }
    }
}
