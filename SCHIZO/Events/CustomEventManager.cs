using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SCHIZO.Helpers;
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

    public CustomEvent GetEvent(string eventName)
    {
        if (eventName is null)
            throw new ArgumentNullException(nameof(eventName));
        if (Events.TryGetValue(eventName, out Type eventType))
            return gameObject.GetComponent(eventType) as CustomEvent;
        return null;
    }

    public T GetEvent<T>() where T : CustomEvent
        => gameObject.GetComponent<T>();

    public CustomEvent EnsureEvent(string eventName)
    {
        if (eventName is null)
            throw new ArgumentNullException(nameof(eventName));
        if (Events.TryGetValue(eventName, out Type eventType))
            return gameObject.EnsureComponent(eventType) as CustomEvent;
        return null;
    }

    public T EnsureEvent<T>() where T : CustomEvent
        => gameObject.EnsureComponent<T>();

    public void AddEvent<T>()
        where T : CustomEvent, new()
    {
        string eventName = typeof(T).Name;
        if (Events.ContainsKey(eventName))
        {
            LOGGER.LogWarning($"Event {eventName} already registered");
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
            Output($"Events: {string.Join(", ", Events.Keys)}");
            return;
        }

        string eventName = (string) n.data[0];
        if (!Events.TryGetValue(eventName, out Type eventType))
        {
            Output($"Event '{eventName}' not found, use \"event\" to list events");
            return;
        }

        Component eventComp = gameObject.GetComponent(eventType);
        if (eventComp is not CustomEvent evt)
        {
            LOGGER.LogError($"Event '{eventName}' has component of wrong type");
            return;
        }

        if (n.data.Count == 1)
        {
            Output($"Event '{eventName}' is {(evt.IsOccurring ? "" : "not ")}occurring");
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
            Output("Syntax: event [name] [start|end]");
            return;
        }

        if (evt.IsOccurring == isStart)
        {
            Output($"Event '{eventName}' is already {(evt.IsOccurring ? "" : "not ")}occurring");
            return;
        }

        if (isStart)
            evt.StartEvent();
        else
            evt.EndEvent();
        Output($"Event '{eventName}' {(isStart ? "start" : "end")}ed");
    }

    private void Output(string msg)
        => MessageHelper.WriteCommandOutput(msg);
}
