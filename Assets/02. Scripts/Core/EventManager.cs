using System;
using System.Collections.Generic;

public static class EventManager
{
    // NOTE : 두 방식 모두 딕셔너리로 관리
    static readonly Dictionary<EGameEvent, object> events = new();

    public static PandaEvent<T> GetEvent<T>(EGameEvent eventType)
    {
        if (events.TryGetValue(eventType, out var e) && e is PandaEvent<T> typedEvent)
        {
            return typedEvent;
        }

        var newEvent = new PandaEvent<T>();
        events[eventType] = newEvent;
        return newEvent;
    }

    // NOTE : Non-Generic 이벤트
    public static PandaEvent GetEvent(EGameEvent eventType)
    {
        if (events.TryGetValue(eventType, out var e) && e is PandaEvent typedEvent)
        {
            return typedEvent;
        }

        var newEvent = new PandaEvent();
        events[eventType] = newEvent;
        return newEvent;
    }

    public static Dictionary<EGameEvent, object> GetSubscribedEvents()
    {
        return events;
    }

    public static void ClearEvents()
    {
        events.Clear();
    }

    public static Action<EGameState> OnGameStateChanged;
    static EGameState gameStatus;

    public static EGameState GameStatus
    {
        get => gameStatus;
        set
        {
            gameStatus = value;
            OnGameStateChanged?.Invoke(gameStatus);
        }
    }
}

public class PandaEvent
{
    private event Action _event;

    public void Subscribe(Action subscriber)
    {
        _event += subscriber;
    }

    public void Unsubscribe(Action subscriber)
    {
        _event -= subscriber;
    }

    public void Clear()
    {
        _event = null;
    }

    public void Invoke()
    {
        _event?.Invoke();
    }
}