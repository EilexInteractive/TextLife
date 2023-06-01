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

    public void AddEvent(LifeEventLog eventLog, WorldEventMethods eventMethods, bool dispatch = true)
    {
        
        if(eventLog != null)
        {
            _EventsOcurred.Add(eventLog);
            switch(eventLog.Type)
            {
                case ELifeEventType.WORLD_WAR_START:
                    eventMethods.CountryWarStart(eventLog.Type, this, ref eventLog);
                    break;
                case ELifeEventType.WORLD_WAR_UPDATE:
                    eventMethods.CountryWarUpdate(eventLog.Type, this, ref eventLog);
                    break;
                case ELifeEventType.WORLD_WAR_END:
                    eventMethods.CountryWarEnd(eventLog.Type, this, ref eventLog);
                    break;

            }
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
