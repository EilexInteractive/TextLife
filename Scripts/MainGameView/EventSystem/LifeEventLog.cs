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

    public bool IsBornEvent = false;

    public LifeEventLog()
    {}

    public LifeEventLog(string text, ELifeEventType type, EAgeCategory category, ESex sex)
    {
        Text = text;
        Type = type;
        AgeCategory = category;
        SexCategory = sex;
    }

    public LifeEventLog(int id, string text, ELifeEventType type, EAgeCategory category, ESex sex, int year, int month)
    {
        ID = id;
        Text = text;
        Type = type;
        AgeCategory = category;
        SexCategory = sex;
        Year = year;
        Month = month;
    }

    public LifeEventLog Copy(int id)
    {
        return new LifeEventLog(id, Text, this.Type, this.AgeCategory, this.SexCategory, Year, Month);
    }

    public LifeEventSave CreateEventSave()
    {
        return new LifeEventSave(ID, Text, (int)Type, (int)SexCategory, (int)AgeCategory, IsBornEvent, Year, Month);
    }

    public LifeEventLog Copy()
    {
        return new LifeEventLog(Text, Type, AgeCategory, SexCategory);
    }
}

public enum ELifeEventType
{
    DATE,
    EVENT,
    BIRTH
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

    public LifeEventSave()
    {}

    public LifeEventSave(int id, string text, int type, int sex, int category, bool bornEvent, int year, int month)
    {
        ID = id;
        Text = text;
        this.Type = type;
        Sex = sex;
        AgeCategory = category;
        IsBornEvent = bornEvent;
        Years = year;
        Month = month;
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
        return eventLog;
    }
}