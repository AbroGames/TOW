using System;
using System.Collections.Generic;

namespace TOW.Scripts.KludgeBox.Events;

internal sealed class EventHub
{
    private List<IListener> _listeners = new List<IListener>();

    internal void Publish<T>(T @event) where T : IEvent
    {
        if (@event is not null)
        {
            foreach (var listener in _listeners)
            {
                listener?.Deliver(@event);
            }
        }
    }

    internal ListenerToken Subscribe<T>(Action<T> action) where T : IEvent
    {
        var subscription = new Listener<T>(action);
        _listeners.Add(subscription);
        var token = new ListenerToken(subscription, this);
        return token;
    }

    internal void Unsubscribe(ListenerToken token)
    {
        _listeners.Remove(token.Listener);
    }

    public void Reset()
    {
        _listeners = new List<IListener>();
    }
}