namespace SCHIZO.Creatures.Components;

partial class HurtSoundPlayer : IOnTakeDamage
{
    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (damageInfo.damage == 0) return;
        fmodSounds.Play((FMOD_CustomEmitter) emitter);
    }
}
