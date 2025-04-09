using System;
using System.Collections.Generic;

namespace ChaosBall.Event
{
    public class EventSystem
    {
        private readonly Dictionary<Type, Delegate> _events = new();
        private readonly Dictionary<Type, List<IEvent>> _eventPool = new();
        
        public void Subscribe<T>(Action<T> handler) where T : IEvent
        {
            // string key = GetKey(handler);

            var @event = typeof(T);

            if (_eventPool.TryGetValue(@event, out var eventList))
            {
                foreach (var e in eventList)
                {
                    // handler.Invoke((T)e);
                    Invoker.Instance.DelegateList.Add(() =>
                    {
                        handler.Invoke((T)e);
                    });
                }
                _eventPool[@event].Clear();
            }
            
            if (_events.TryGetValue(@event, out var existHandlers))
            {
                _events[@event] = Delegate.Combine(existHandlers, handler);
            }
            else
            {
                _events[@event] = handler;
            }
        }

        public void Unsubscribe<T>(Action<T> handler) where T : IEvent
        {
            // string key = GetKey(handler);
            var key = typeof(T);
            if (_events.TryGetValue(key, out var existHandlers))
            {
                Delegate newHandlers = Delegate.Remove(existHandlers, handler);
                if (newHandlers == null)
                {
                    _events.Remove(key);
                }
                else
                {
                    _events[key] = newHandlers;
                }
            }
        }

        public void Publish<T>(T e) where T : IEvent
        {
            var @event = typeof(T);
            if (_events.TryGetValue(@event, out var existHandlers))
            {
                // (existHandlers as Action<T>)?.Invoke(e);
                Invoker.Instance.DelegateList.Add(() =>
                {
                    (existHandlers as Action<T>)?.Invoke(e);
                });
            }
            if (_eventPool.TryGetValue(@event, out var eventList))
            {
                eventList.Add(e);
            }
            else
            {
                _eventPool[@event] = new List<IEvent> { e };
            }
        }

        public void Publish<T>() where T : IEvent, new()
        {
            T e = new T();
            var @event = typeof(T);
            if (_events.TryGetValue(@event, out var existHandlers))
            {
                // (existHandlers as Action<T>)?.Invoke(e);
                Invoker.Instance.DelegateList.Add(() =>
                {
                    (existHandlers as Action<T>)?.Invoke(e);
                });
                return;
            }
            if (_eventPool.TryGetValue(@event, out var eventList))
            {
                eventList.Add(e);
            }
            else
            {
                _eventPool[@event] = new List<IEvent> { e };
            }
        }
    }
}