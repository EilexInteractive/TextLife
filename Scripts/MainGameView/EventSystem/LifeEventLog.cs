using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

public class LifeEventLog
{
    public int ID;
    public string Text;
    public ELifeEventType Type;
    public EAgeCategory AgeCategory;
    public ESex SexCategory;
    public int Year;
    public int Month;
    public int Cost;
    public int Tiredness;

    [JsonIgnore]
    public List<EventAction> Actions = new List<EventAction>();

    public bool IsBornEvent = false;

    public LifeEventLog()
    {}

    public LifeEventLog(string text, ELifeEventType type, EAgeCategory category, ESex sex, List<EventAction> actions = null, int tiredness = 0, int cost = 0)
    {
        Text = text;
        Type = type;
        AgeCategory = category;
        SexCategory = sex;
        if(actions != null)
            Actions = actions;

        Cost = cost;
        Tiredness = tiredness;
    }

    public LifeEventLog(int id, string text, ELifeEventType type, EAgeCategory category, ESex sex, int year, int month, List<EventAction> events)
    {
        ID = id;
        Text = text;
        Type = type;
        AgeCategory = category;
        SexCategory = sex;
        Year = year;
        Month = month;
        Actions = events;
    }

    public void Dispatch()
    {
        foreach(var e in Actions)
            e.Dispatch();
    }

    public void Dispatch(string prop)
    {
        foreach(var e in Actions)
            if(e.EventType == EEventActionType.STRING)
                e.Dispatch(prop);
    }

    public void Dispatch(int prop)
    {
        foreach(var e in Actions)
            if(e.EventType == EEventActionType.INT)
                e.Dispatch(prop);
    }

    public void Dispatch(float prop)
    {
        foreach(var e in Actions)
            if(e.EventType == EEventActionType.FLOAT)
                e.Dispatch(prop);
    }

    public void Dispatch(bool prop)
    {
        foreach(var e in Actions)
            if(e.EventType == EEventActionType.BOOL)
                e.Dispatch(prop);
    }

    public void Dispatch(ELifeEventType type, WorldEventData eventData)
    {
        foreach(var e in Actions)
            if(e.EventType == EEventActionType.WORLD_EVENT)
                e.Dispatch(type, eventData, this);
    }

    public void DispatchAll(string propStr, int propIn, float propFl, bool propBo)
    {
        Dispatch();
        Dispatch(propStr);
        Dispatch(propIn);
        Dispatch(propFl);
        Dispatch(propBo);
    }

    public LifeEventLog Copy(int id)
    {
        return new LifeEventLog(id, Text, this.Type, this.AgeCategory, this.SexCategory, Year, Month, Actions);
    }

    public virtual void Perform(CharacterDetails character)
    {
        if(character != null)
        {
            character.Stats?.ReduceTiredness(this.Tiredness);
        }
    }

    public LifeEventSave CreateEventSave()
    {
        return new LifeEventSave(ID, Text, (int)Type, (int)SexCategory, (int)AgeCategory, IsBornEvent, Year, Month,  Tiredness, Cost);
    }

    public LifeEventLog Copy()
    {
        return new LifeEventLog(Text, Type, AgeCategory, SexCategory, Actions);
    }
}

public enum ELifeEventType
{
    DATE = 0,
    EVENT = 1,
    BIRTH = 2,
    WORLD_WAR_START = 3,
    WORLD_WAR_UPDATE = 4,
    WORLD_WAR_END = 5,
    RELATIONSHIP_EVENT = 6
}

public class LifeEventSave
{
    [JsonProperty]
    public int ID;
    [JsonProperty]
    public string Text;
    [JsonProperty]
    public int Type;
    [JsonProperty]
    public int Sex;
    [JsonProperty]
    public int AgeCategory;
    [JsonProperty]
    public bool IsBornEvent;
    [JsonProperty]
    public int Years;
    [JsonProperty]
    public int Month;
    public int Cost;
    public int Tiredness;

    public LifeEventSave()
    {}

    public LifeEventSave(int id, string text, int type, int sex, int category, bool bornEvent, int year, int month, int cost, int tired)
    {
        ID = id;
        Text = text;
        this.Type = type;
        Sex = sex;
        AgeCategory = category;
        IsBornEvent = bornEvent;
        Years = year;
        Month = month;
        Tiredness = tired;
    }

    public LifeEventLog LoadLifeEvent()
    {
        LifeEventLog eventLog = new LifeEventLog();
        eventLog.ID = this.ID;
        eventLog.Text = this.Text;
        eventLog.Type = (ELifeEventType)this.Type;
        eventLog.SexCategory = (ESex)Sex;
        eventLog.AgeCategory = (EAgeCategory)this.AgeCategory;
        eventLog.IsBornEvent = this.IsBornEvent;
        eventLog.Tiredness = this.Tiredness;
        eventLog.Cost = this.Cost;
        return eventLog;
    }
}