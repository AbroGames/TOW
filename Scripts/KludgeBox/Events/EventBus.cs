﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace TOW.Scripts.KludgeBox.Events;

/// <summary>
/// Provides a central event bus for publishing and subscribing to events.
/// </summary>
public class EventBus
{
    /// <summary>
    /// If set to true, EventBus will attempt to publish events to all EventHubs whose types are derived from the event type.
    /// This option can significantly impact performance.
    /// </summary>
    public bool IncludeBaseEvents = true;

    private Dictionary<Type, EventHub> _hubs = new Dictionary<Type, EventHub>();

    /// <summary>
    /// Subscribes a listener to the specified event type.
    /// </summary>
    /// <typeparam name="T">The event type to subscribe to.</typeparam>
    /// <param name="action">The action to execute when the event is published.</param>
    /// <returns>A listener token that can be used to unsubscribe from the event.</returns>
    public ListenerToken Subscribe<T>(Action<T> action) where T : IEvent
    {
        return GetHub(typeof(T)).Subscribe(action);
    }

    /// <summary>
    /// Publishes an event to all registered listeners.
    /// </summary>
    /// <typeparam name="T">The type of event to publish.</typeparam>
    /// <param name="event">The event to publish.</param>
    public void Publish<T>(T @event) where T : IEvent
    {
        if (IncludeBaseEvents)
        {
            foreach (var hub in FindApplicableHubs(typeof(T)))
            {
                hub.Publish(@event);
            }
        }
        else
        {
            GetHub(typeof(T)).Publish(@event);
        }
    }

    /// <summary>
    /// Resets all the EventHubs.
    /// </summary>
    public void Reset()
    {
        _hubs.Clear();
    }

    /// <summary>
    /// Resets the EventHub associated with the specified event type.
    /// </summary>
    /// <typeparam name="T">The event type for which to reset the EventHub.</typeparam>
    public void ResetEvent<T>() where T : IEvent
    {
        GetHub(typeof(T)).Reset();
    }

    private EventHub GetHub(Type eventType)
    {
        if (_hubs.TryGetValue(eventType, out EventHub? hub) && hub is not null)
        {
            return hub;
        }

        hub = new EventHub();
        _hubs[eventType] = hub;

        return hub;
    }
    
    /// <summary>
    ///	Subscribes to a message type using the provided MethodInfo.
    /// </summary>
    /// <param name="methodInfo">The MethodInfo representing the delivery action.</param>
    /// <returns>Message subscription token that can be used for unsubscribing.</returns>
    public ListenerToken SubscribeMethod(MethodInfo methodInfo)
    {
        Type messageType = methodInfo.GetParameters()[0].ParameterType;

        // Create an Action<TArg> delegate from the MethodInfo
        var delegateType = typeof(Action<>).MakeGenericType(messageType);
        var actionDelegate = Delegate.CreateDelegate(delegateType, null, methodInfo);

        // Subscribe to the message type using the created delegate
        return typeof(EventBus).GetMethod("Subscribe")!.MakeGenericMethod(messageType)
            .Invoke(this, new object[] { actionDelegate }) as ListenerToken;
    }

    private List<EventHub> FindApplicableHubs(Type eventType)
    {
        List<EventHub> applicableHubs = new List<EventHub>();

        foreach (var kv in _hubs)
        {
            if (kv.Key.IsAssignableFrom(eventType))
            {
                applicableHubs.Add(kv.Value);
            }
        }

        if (applicableHubs.Count == 0)
        {
            applicableHubs.Add(GetHub(eventType));
        }

        return applicableHubs;
    }
}