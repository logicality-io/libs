using System;
using System.Collections.Generic;

namespace Logicality.EventSourcing.Domain;

public class EventPlayer
{
    private readonly Dictionary<Type, Action<object>> _handlers
        = new Dictionary<Type, Action<object>>();

    public void Register<TEvent>(Action<TEvent> handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        var typeOfEvent = typeof(TEvent);
        if (_handlers.ContainsKey(typeOfEvent))
        {
            throw new InvalidOperationException(
                "There's already a handler registered for the event of type " +
                $"'{typeOfEvent.Name}'");
        }

        _handlers.Add(typeOfEvent, @event => handler((TEvent)@event));
    }

    public void Register(Type typeOfEvent, Action<object> handler)
    {
        if (typeOfEvent == null)
        {
            throw new ArgumentNullException(nameof(typeOfEvent));
        }

        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        if (_handlers.ContainsKey(typeOfEvent))
        {
            throw new InvalidOperationException(
                "There's already a handler registered for the event of type " +
                $"'{typeOfEvent.Name}'");
        }

        _handlers.Add(typeOfEvent, handler);
    }

    public void Play(object @event)
    {
        if (@event == null)
        {
            throw new ArgumentNullException(nameof(@event));
        }

        if (_handlers.TryGetValue(@event.GetType(), out var handler))
        {
            handler(@event);
        }
    }
}