using System;
using UnityEngine;

namespace SCHIZO.Events
{
    public abstract class CustomEvent : MonoBehaviour
    {
        public abstract bool IsOccurring { get; }

        public event Action Started;
        public event Action Ended;
        public virtual void StartEvent()
        {
            LOGGER.LogDebug($"{GetType().Name} started");
            Started?.Invoke();
        }
        public virtual void EndEvent()
        {
            LOGGER.LogDebug($"{GetType().Name} ended");
            Ended?.Invoke();
        }


        protected abstract void UpdateLogic();
        protected abstract void UpdateRender();
        protected abstract bool ShouldStartEvent();

        private void FixedUpdate()
        {
            if (IsOccurring)
                UpdateLogic();
            else
                if (ShouldStartEvent()) StartEvent();
        }

        private void Update()
        {
            if (IsOccurring)
                UpdateRender();
        }

        private void OnDisable()
        {
            EndEvent();
        }
    }
}
