using System;
using System.Collections.Generic;
using Godot;

public class EventAction
{
    public string EventName;
    public LifeEventLog EventLog;

    public EEventActionType EventType;

    public event Action EventActionNoProps;
    public event Action<int> EventActionInt;
    public event Action<string> EventActionString;
    public event Action<float> EventActionFloat;
    public event Action<bool> EventActionBool;
    public event Action<ELifeEventType, WorldEventData, LifeEventLog> EventActionWorldEvent;

    public void Dispatch() => EventActionNoProps?.Invoke();
    public void Dispatch(string prop) => EventActionString?.Invoke(prop);
    public void Dispatch(int prop) => EventActionInt?.Invoke(prop);
    public void Dispatch(float prop) => EventActionFloat?.Invoke(prop);
    public void Dispatch(bool prop) => EventActionBool?.Invoke(prop);
    public void Dispatch(ELifeEventType type, WorldEventData eventData, LifeEventLog log) => EventActionWorldEvent?.Invoke(type, eventData, log);

    public void DispatchAll(string propString, int propInt, float propFloat, bool propBool)
    {
        Dispatch();
        Dispatch(propString);
        Dispatch(propInt);
        Dispatch(propFloat);
        Dispatch(propBool);
    }

    public static bool operator ==(string a, EventAction b)
    {
        if(a == b.EventName)
            return true;

        return false;
    }

    public static bool operator !=(string a, EventAction b)
    {
        return a != b;
    }

    
}