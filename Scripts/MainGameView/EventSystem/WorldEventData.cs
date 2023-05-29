using System;
using System.Collections.Generic;
using Godot;

public class WorldEventData
{
    private Country _CountryOrigin;
    public Country CountryOrigin => _CountryOrigin;
    private Country _InvolvedCountry;
    public Country InvolvedCountry => _InvolvedCountry;
    private ELifeEventType _EventType;
    
    private List<LifeEventLog> _EventsOcurred = new List<LifeEventLog>();
    public List<LifeEventLog> EventsOcurred { get => _EventsOcurred; }

    public WorldEventData(Country origin, Country involved = null)
    {
        _CountryOrigin = origin;
        _InvolvedCountry = origin;
    }

    public void AddEvent(LifeEventLog eventLog, bool dispatch = true)
    {
        if(eventLog != null)
        {
            _EventsOcurred.Add(eventLog);
            if(dispatch)
                eventLog.Dispatch(_EventType, this);
        }
    }

    public bool hasEventOcurred(LifeEventLog lifeEvent)
    {
        foreach(var e in _EventsOcurred)
        {
            if(e.Text == lifeEvent.Text)
                return true;
        }

        return false;
    }

    public void SetEventType(ELifeEventType type) => _EventType = type;
    public ELifeEventType GetEventType() => _EventType;
}
