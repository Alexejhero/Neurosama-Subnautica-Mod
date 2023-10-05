namespace SCHIZO.Events
{
    public abstract class CustomEvent : MonoBehaviour
    {
        public abstract bool IsOccurring { get; }

        public virtual void StartEvent()
        {
            LOGGER.LogDebug($"{GetType().Name} started");
        }

        public virtual void EndEvent()
        {
            LOGGER.LogDebug($"{GetType().Name} ended");
        }

        protected abstract void UpdateLogic();
        protected abstract void UpdateRender();
        protected abstract bool ShouldStartEvent();

        private void FixedUpdate()
        {
            if (IsOccurring) UpdateLogic();
            else if (CustomEventManager.Instance.EnableAutoEvents && ShouldStartEvent()) StartEvent();
        }

        private void Update()
        {
            if (IsOccurring) UpdateRender();
        }

        private void OnDisable()
        {
            if (IsOccurring) EndEvent();
        }
    }
}
