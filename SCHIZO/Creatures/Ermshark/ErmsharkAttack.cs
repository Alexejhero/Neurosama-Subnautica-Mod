using SCHIZO.Creatures.Tutel;
using SCHIZO.Sounds;
using UnityEngine;

namespace SCHIZO.Creatures.Ermshark;

public sealed class ErmsharkAttack : MeleeAttack
{
    private SoundPlayer _attackSounds;

    private void Start()
    {
        _attackSounds = CreatureSoundsHandler.GetCreatureSounds(ModItems.Ermshark).AttackSounds;
    }

    public override void OnTouch(Collider collider)
    {
        if (!enabled) return;
        if (!liveMixin.IsAlive()) return;

        GameObject target = GetTarget(collider);

        if (CreatureData.GetCreatureType(gameObject) == CreatureData.GetCreatureType(target)) return;
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
                    creature.GetComponent<BullyTutel>().TryPickupTutel(heldTutel);
                    creature.SetFriend(player.gameObject, 120f);
                    return;
                }
                else if (canBeFed && player.CanBeAttacked() && TryEat(heldObject, true))
                {
                    if (attackSound) Utils.PlayEnvSound(attackSound, mouth.transform.position);
                    gameObject.SendMessage("OnMeleeAttack", heldObject, SendMessageOptions.DontRequireReceiver);
                    return;
                }
            }
        }

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
            if (attackSound != null)
            {
                Utils.PlayEnvSound(attackSound, damageFxPos);
            }

            _attackSounds.Play(gameObject.GetComponent<FMOD_CustomEmitter>());

            creature.Aggression.Add(-biteAggressionDecrement);
            if (living && !living.IsAlive())
            {
                TryEat(living.gameObject);
            }
            gameObject.SendMessage("OnMeleeAttack", target, SendMessageOptions.DontRequireReceiver);
        }
    }
}
