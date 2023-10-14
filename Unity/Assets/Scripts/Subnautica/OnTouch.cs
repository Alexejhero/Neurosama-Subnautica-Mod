using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof (Collider))]
public class OnTouch : MonoBehaviour
{
    public string tagFilter;
    public OnTouchEvent onTouch;

    [Serializable]
    public class OnTouchEvent : UnityEvent<Collider>
    {
    }
}
