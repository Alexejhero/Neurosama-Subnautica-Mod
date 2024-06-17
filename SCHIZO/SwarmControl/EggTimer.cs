using System;
using System.Collections;
using UnityEngine;
using UWE;

namespace SCHIZO.SwarmControl;

#nullable enable
internal class EggTimer : IDisposable
{
    private readonly Coroutine _timerCoro;
    private readonly Action _onStart;
    private readonly Action _onEnd;
    private readonly Action? _onTick;
    private bool _active;
    private float _timeLeft;

    public bool IsActive => _active;
    public float TimeLeft => _timeLeft;

    public EggTimer(Action onStart, Action onEnd, Action? onTick = null)
    {
        _onStart = onStart;
        _onEnd = onEnd;
        _onTick = onTick;
        _timerCoro = CoroutineHost.StartCoroutine(TimerCoro());
    }

    public void AddTime(float seconds)
    {
        _timeLeft += seconds;
        if (_timeLeft > 0)
            Start();
    }

    private IEnumerator TimerCoro()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (!_active) continue;

            _onTick?.Invoke();
            _timeLeft -= Time.fixedDeltaTime;
            if (_timeLeft <= 0)
            {
                _timeLeft = 0;
                End();
            }
        }
    }

    public void Start()
    {
        if (_active) return;
        _active = true;
        _onStart?.Invoke();
    }
    public void End()
    {
        if (!_active) return;
        _active = false;
        _onEnd?.Invoke();
    }

    public void Dispose()
    {
        CoroutineHost.StopCoroutine(_timerCoro);
    }
}
