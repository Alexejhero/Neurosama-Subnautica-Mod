using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace SCHIZO.Events;

public class CustomEventManager : MonoBehaviour
{
    public static CustomEventManager main;

    private readonly Dictionary<string, Type> Events = new(StringComparer.InvariantCultureIgnoreCase);

    public void Awake()
    {
        main = this;
        DevConsole.RegisterConsoleCommand(this, "event");
        DevConsole.RegisterConsoleCommand(this, "events");
    }

    public void AddEvent<T>(string eventName = null)
        where T : ICustomEvent, new()
    {
        eventName ??= new T().Name;
        if (Events.ContainsKey(eventName))
        {
            Debug.LogError($"Event {eventName} already registered");
            return;
        }
        Type evt = typeof(T);
        Events.Add(eventName, evt);
        gameObject.EnsureComponent(evt);
    }

    public void RemoveEvent(string eventName)
    {
        if (Events.TryGetValue(eventName, out Type eventType))
        {
            Events.Remove(eventName);
            Component comp = gameObject.GetComponent(eventType);
            Destroy(comp);
        }
    }

    [UsedImplicitly]
    private void OnConsoleCommand_events(NotificationCenter.Notification n)
        => OnConsoleCommand_event(n);

    private void OnConsoleCommand_event(NotificationCenter.Notification n)
    {
        if (n?.data?.Count is null or 0)
        {
            ErrorMessage.AddDebug($"Events: {string.Join(", ", Events.Keys)}");
            return;
        }

        string eventName = (string) n.data[0];
        if (!Events.TryGetValue(eventName, out Type eventType))
        {
            ErrorMessage.AddDebug($"Event '{eventName}' not found, use \"event\" to list events");
            return;
        }

        Component eventComp = gameObject.GetComponent(eventType);
        if (eventComp is not ICustomEvent evt)
        {
            Debug.LogError($"Event '{eventName}' has component of wrong type");
            return;
        }

        if (n.data.Count == 1)
        {
            ErrorMessage.AddDebug($"Event '{eventName}' is {(evt.IsOccurring ? "" : "not ")}occurring");
            return;
        }

        string startOrEndArg = (string) n.data[1];
        bool? isStartMaybe = startOrEndArg switch
        {
            "start" or "1" or "on" or "true" or "play" or "start" => true,
            "end" or "0" or "off" or "false" or "stop" => false,
            _ => null
        };
        if (isStartMaybe is not { } isStart)
        {
            ErrorMessage.AddDebug("Syntax: event [name] [start|end]");
            return;
        }

        if (evt.IsOccurring == isStart)
        {
            ErrorMessage.AddDebug($"Event '{eventName}' is already {(evt.IsOccurring ? "" : "not ")}occurring");
            return;
        }

        if (isStart)
            evt.StartEvent();
        else
            evt.EndEvent();
        ErrorMessage.AddDebug($"Event '{eventName}' {(isStart ? "start" : "end")}ed");
    }
}
