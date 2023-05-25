using System;
using System.Collections.Generic;
using Godot;

public class EventAction
{
    public string EventName;
    public LifeEventLog EventLog;

    public event Action EventActionNoProps;
    public event Action<int> EventActionInt;
    public event Action<string> EventActionString;
    public event Action<float> EventActionFloat;
    public event Action<bool> EventActionBool;

    public void Dispatch<T>(T property)
    {
        if(property is string propString)
        {
            if(propString == "")
            {
                EventActionNoProps?.Invoke();
            } else 
            {
                EventActionString?.Invoke(propString);
            }
        } else if(property is int propInt)
        {
            EventActionInt?.Invoke(propInt);
        } else if(property is float propFloat)
        {
            EventActionFloat?.Invoke(propFloat);
        } else if(property is bool propBool)
        {
            EventActionBool?.Invoke(propBool);
        } else 
        {
            EventActionNoProps?.Invoke();
        }
    }

    public void DispatchAll(string propString, int propInt, float propFloat, bool propBool)
    {
        Dispatch<string>("");
        Dispatch<string>(propString);
        Dispatch<int>(propInt);
        Dispatch<float>(propFloat);
        Dispatch<bool>(propBool);
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