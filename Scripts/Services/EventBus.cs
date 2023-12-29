﻿using System;
using Godot;
using TOW.Scripts.KludgeBox.Events;
using TOW.Scripts.Services.ModLoader;
using Bus = TOW.Scripts.KludgeBox.Events.EventBus;

namespace TOW.Scripts.Services;


[GlobalClass]
public partial class EventBus : Service
{
    private Bus _bus = new Bus();

    public EventBus()
    {
        _bus.IncludeBaseEvents = false;
    }

    public virtual void ScanEventListeners()
    {
        var listeners = EventScanner.ScanEventListeners();
        _bus.SubscribeMethods(listeners);
    }

    /// <summary>
    /// Subscribes a listener to the specified event type.
    /// </summary>
    /// <typeparam name="T">The event type to subscribe to.</typeparam>
    /// <param name="action">The action to execute when the event is published.</param>
    /// <returns>A listener token that can be used to unsubscribe from the event.</returns>
    public virtual ListenerToken Subscribe<T>(Action<T> listener) where T : IEvent
    {
        return _bus.Subscribe(listener);
    }

    /// <summary>
    /// Publishes an event to all registered listeners.
    /// </summary>
    /// <typeparam name="T">The type of event to publish.</typeparam>
    /// <param name="event">The event to publish.</param>
    public virtual void Publish<T>(T @event) where T : IEvent
    {
        _bus.Publish(@event);
    }

    /// <summary>
    /// Publishes a cancellable event using the message bus and returns a value indicating whether the event was cancelled.
    /// </summary>
    /// <typeparam name="T">The type of the cancellable event. It must inherit from <see cref="CancellableEvent"/>.</typeparam>
    /// <param name="event">The cancellable event to be published.</param>
    /// <returns>
    ///   <c>true</c> if the event was cancelled; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    ///   <para>
    ///   This method publishes a cancellable event using the message bus and checks whether the event has been cancelled.
    ///   </para>
    ///   <para>
    ///   The event type must be a subclass of <see cref="CancellableEvent"/>.
    ///   </para>
    /// </remarks>
    public bool PublishCancellable<T>(T @event) where T : CancellableEvent
    {
        _bus.Publish(@event);
        return @event.IsCancelled;
    }
    
    /// <summary>
    /// Resets all the EventHubs.
    /// </summary>
    public virtual void Reset()
    {
        _bus.Reset();
    }

    /// <summary>
    /// Resets the EventHub associated with the specified event type.
    /// </summary>
    /// <typeparam name="T">The event type for which to reset the EventHub.</typeparam>
    public virtual void ResetEvent<T>() where T : IEvent
    {
        _bus.ResetEvent<T>();
    }

    internal override void Run()
    {
        ScanEventListeners();
    }
}