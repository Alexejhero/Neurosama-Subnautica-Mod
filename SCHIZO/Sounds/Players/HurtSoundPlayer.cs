namespace SCHIZO.Sounds.Players;

partial class HurtSoundPlayer : IOnTakeDamage
{
    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (damageInfo.damage != 0) Play();
    }
}
