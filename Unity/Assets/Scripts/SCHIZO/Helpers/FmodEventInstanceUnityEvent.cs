using System;
using UnityEngine.Events;
using FMOD.Studio;

namespace SCHIZO.Helpers
{
    // https://docs.unity3d.com/ScriptReference/Events.UnityEvent_1.html
    [Serializable]
    public class FmodEventInstanceUnityEvent : UnityEvent<EventInstance>
    {
    }
}
