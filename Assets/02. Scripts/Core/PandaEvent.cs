using System;

public class PandaEvent<T>
{
    event Action<T> _event;

    public void Subscribe(Action<T> subscriber)
    {
        _event += subscriber;
    }

    public void Unsubscribe(Action<T> subscriber)
    {
        _event -= subscriber;
    }

    public void Invoke(T arg)
    {
        _event?.Invoke(arg);
    }
}
