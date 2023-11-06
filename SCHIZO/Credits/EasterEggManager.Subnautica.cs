using UnityEngine;

namespace SCHIZO.Credits;

partial class EasterEggManager
{
    private EndCreditsManager _credits;
    private bool _easterEggAdjusted;

    private void Awake()
    {
        _credits = GetComponentInParent<EndCreditsManager>();
    }

    private void Update()
    {
        if (_credits.phase != EndCreditsManager.Phase.Easter || EndCreditsManager.showEaster) return;

        if (!_easterEggAdjusted)
        {
            float startTime = _credits.phaseStartTime - Time.unscaledTime;

            sounds.Play2D(0, startTime);
            sounds.Play2D(1, startTime + 1.5f);
            sounds.Play2D(2, startTime + 3.0f);
            sounds.Play2D(3, startTime + 3.5f);

            // it's actually the end time (just for the easter egg phase)
            _credits.phaseStartTime += 5f;

            _easterEggAdjusted = true;
        }
    }

    public void Reset()
    {
        _easterEggAdjusted = false;
    }
}
