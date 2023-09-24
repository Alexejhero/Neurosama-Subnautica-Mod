using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCHIZO.Creatures.Tutel;

public sealed class TutelBehaviour : CaveCrawler
{
    private enum Emotion
    {
        Normal,
        Smug,
        Glad,
        Susge,
        Sad,
        Pog
    }

    private Animator _animator;

    private Emotion _currentEmotion = Emotion.Normal;
    private readonly Queue<Emotion> _nextEmotions = new();
    private Coroutine _emotionSwitchCoroutine;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    private void LateUpdate()
    {
        if (_nextEmotions.Count > 0 && _emotionSwitchCoroutine == null)
        {
            _emotionSwitchCoroutine = StartCoroutine(SwitchEmotions(_nextEmotions.Dequeue()));
        }
    }

    private void SetEmotion(Emotion emotion)
    {
        _nextEmotions.Enqueue(emotion);
    }

    private IEnumerator SwitchEmotions(Emotion newEmotion)
    {
        int currentEyesLayer = _animator.GetLayerIndex($"{_currentEmotion.ToString()}/eyes");
        int currentMouthLayer = _animator.GetLayerIndex($"{_currentEmotion.ToString()}/mouth");

        int nextEyesLayer = _animator.GetLayerIndex($"{newEmotion.ToString()}/eyes");
        int nextMouthLayer = _animator.GetLayerIndex($"{newEmotion.ToString()}/mouth");

        const float transitionDuration = 0.25f;
        for (float f = 0; f < transitionDuration; f += Time.deltaTime)
        {
            _animator.SetLayerWeight(currentEyesLayer, 1 - f / transitionDuration);
            _animator.SetLayerWeight(currentMouthLayer, 1 - f / transitionDuration);
            _animator.SetLayerWeight(nextEyesLayer, f / transitionDuration);
            _animator.SetLayerWeight(nextMouthLayer, f / transitionDuration);

            yield return null;
        }

        _animator.SetLayerWeight(currentEyesLayer, 0);
        _animator.SetLayerWeight(currentMouthLayer, 0);
        _animator.SetLayerWeight(nextEyesLayer, 1);
        _animator.SetLayerWeight(nextMouthLayer, 1);

        _currentEmotion = newEmotion;

        _emotionSwitchCoroutine = null;
    }
}
