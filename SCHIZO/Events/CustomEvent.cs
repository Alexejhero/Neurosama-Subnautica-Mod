using UnityEngine;

namespace SCHIZO.Events
{
    public abstract class CustomEvent : MonoBehaviour
    {
        public abstract bool IsOccurring { get; }
        public abstract void StartEvent();
        public abstract void EndEvent();
    }
}
