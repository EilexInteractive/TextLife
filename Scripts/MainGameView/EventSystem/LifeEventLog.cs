using System;
using System.Collections.Generic;
using Godot;

public class LifeEventLog
{
    public int ID;
    public string Text;
    public ELifeEventType Type;
    public EAgeCategory AgeCategory;
    public ESex SexCategory;

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

    public LifeEventLog(int id, string text, ELifeEventType type, EAgeCategory category, ESex sex)
    {
        ID = id;
        Text = text;
        Type = type;
        AgeCategory = category;
        SexCategory = sex;
    }

    public LifeEventLog Copy(int id)
    {
        return new LifeEventLog(id, Text, this.Type, this.AgeCategory, this.SexCategory);
    }

    public LifeEventSave CreateEventSave()
    {
        return new LifeEventSave(ID, Text, (int)Type, (int)SexCategory, (int)AgeCategory, IsBornEvent);
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
    public int ID;
    public string Text;
    public int Type;
    public int Sex;
    public int AgeCategory;
    public bool IsBornEvent;
    public LifeEventSave()
    {}

    public LifeEventSave(int id, string text, int type, int sex, int category, bool bornEvent)
    {
        ID = id;
        Text = text;
        this.Type = type;
        Sex = sex;
        AgeCategory = category;
        IsBornEvent = bornEvent;
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