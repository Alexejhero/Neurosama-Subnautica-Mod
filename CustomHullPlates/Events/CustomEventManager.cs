using System;
using System.Collections.Generic;
using UnityEngine;

namespace SCHIZO.Events;
public class CustomEventManager : MonoBehaviour
{
    public CustomEventManager main;
    private readonly Dictionary<string, Type> Events = new(StringComparer.InvariantCultureIgnoreCase);

    public void Awake()
    {
        main = this;
        DevConsole.RegisterConsoleCommand(this, "event");
    }

    public void AddEvent<T>(string name = null)
        where T : ICustomEvent, new()
    {
        name ??= new T().Name;
        if (Events.ContainsKey(name))
        {
            Debug.LogError($"Event {name} already registered");
            return;
        }
        var evt = typeof(T);
        Events.Add(name, evt);
        gameObject.EnsureComponent(evt);
    }

    public void RemoveEvent(string name)
    {
        if (Events.TryGetValue(name, out var eventType))
        {
            Events.Remove(name);
            var comp = gameObject.GetComponent(eventType);
            Destroy(comp);
        }
    }

    private void OnConsoleCommand_event(NotificationCenter.Notification n)
    {
        if (n?.data?.Count is null or 0)
        {
            ErrorMessage.AddDebug($"Events: {string.Join(", ", Events.Keys)}");
            return;
        }
        string eventName = n.data[0] as string;
        if (!Events.TryGetValue(eventName, out var eventType))
        {
            ErrorMessage.AddDebug($"Event '{eventName}' not found, use \"event\" to list events");
            return;
        }
        var eventComp = gameObject.GetComponent(eventType);
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
        var startOrEndArg = n.data[1] as string;
        bool? isStartMaybe = startOrEndArg switch
        {
            "start" or "1" or "on" => true,
            "end" or "0" or "off" => false,
            _ => null
        };
        if (isStartMaybe is bool isStart)
        {
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
        else
        {
            ErrorMessage.AddDebug("Syntax: event [name] [start|end]");
        }
    }
}
