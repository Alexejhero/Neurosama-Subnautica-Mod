using SCHIZO.Helpers;
using UnityEngine;

namespace SCHIZO.Credits;

partial class EasterEggPlayer
{
    private EndCreditsManager _credits;
    private bool _easterEggAdjusted;

    private void Awake()
    {
        _credits = GetComponentInParent<EndCreditsManager>();
    }

    private float _startTime;
    private void Update()
    {
#if DEBUG_CREDITS
        if (_credits.phase < EndCreditsManager.Phase.Easter && !EndCreditsManager.showEaster)
            EndCreditsManager.showEaster = true;
#endif
        if (_credits.phase < EndCreditsManager.Phase.Easter || EndCreditsManager.showEaster) return;

        if (!_easterEggAdjusted)
        {
            // it's actually the end time (just for the easter egg phase)
            _startTime = _credits.phaseStartTime;
            _credits.phaseStartTime += FMODExtensions.GetLength(soundEvent) / 1000f;

            _easterEggAdjusted = true;
        }
        else
        {
            if (Time.unscaledTime > _startTime)
            {
                PlayOneShot();
                _startTime = float.PositiveInfinity;
            }
        }
    }

    public void Reset()
    {
        _easterEggAdjusted = false;
    }
}
