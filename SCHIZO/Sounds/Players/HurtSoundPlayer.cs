namespace SCHIZO.Sounds.Players;

partial class HurtSoundPlayer : IOnTakeDamage
{
    private string _hurtSound;
    private void Awake()
    {
        _hurtSound = soundEvent;
    }
    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (damageInfo.damage <= 0) return;
        soundEvent = _hurtSound;
        Play(0.01f);
    }
    public void OnKill()
    {
        CancelAllDelayed();
        soundEvent = deathSound;
        Play();
    }
}
