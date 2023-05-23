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
}

public enum ELifeEventType
{
    DATE,
    EVENT,
    BIRTH
}