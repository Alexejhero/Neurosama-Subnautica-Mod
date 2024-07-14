using System;
using System.Collections;
using UnityEngine;
using UWE;

namespace SCHIZO.SwarmControl;

#nullable enable
internal class EggTimer : IDisposable
{
    private readonly Coroutine _timerCoro;
    private readonly Action? _onStart;
    private readonly Action? _onEnd;
    private readonly Action? _onTick;

    public bool IsActive { get; private set; }
    public float TimeLeft { get; private set; }

    public EggTimer(Action onStart, Action onEnd, Action? onTick = null)
    {
        _onStart = onStart;
        _onEnd = onEnd;
        _onTick = onTick;
        _timerCoro = CoroutineHost.StartCoroutine(TimerCoro());
    }

    public void AddTime(float seconds)
    {
        TimeLeft += seconds;
        if (TimeLeft > 0)
            Start();
    }

    private IEnumerator TimerCoro()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (!IsActive) continue;

            _onTick?.Invoke();
            TimeLeft -= Time.fixedDeltaTime;
            if (TimeLeft <= 0)
            {
                TimeLeft = 0;
                End();
            }
        }
    }

    public void Start()
    {
        if (IsActive) return;
        IsActive = true;
        _onStart?.Invoke();
    }
    public void End()
    {
        if (!IsActive) return;
        IsActive = false;
        _onEnd?.Invoke();
    }

    public void Dispose()
    {
        CoroutineHost.StopCoroutine(_timerCoro);
    }
}
