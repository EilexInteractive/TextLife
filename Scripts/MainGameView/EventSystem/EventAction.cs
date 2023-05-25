using System;
using Godot;

public class EventAction
{
    public string EventName;
    public LifeEventLog EventLog;
    public event Action ActionEvent;
    public event Action<int> ActionEventInt;
    public event Action<string> ActionEventString;
    public event Action<float> ActionEventFloat;
    public event Action<bool> ActionEventBool;
}