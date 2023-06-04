using System;
using Godot;
using System.Collections.Generic;


public class CharacterSave
{
    public string CharacterID;
    public string FirstName;
    public string LastName;
    public int Sex;

    public int YearsOld;
    public int MonthsOld;
    public string CountryName;
    public string State;

    public List<LifeEventSave> DateLog = new List<LifeEventSave>();
    public List<LifeEventSave> EventLog = new List<LifeEventSave>();
    public List<LifeEventRequest> RequestEvents = new List<LifeEventRequest>();

    public CharacterSave()
    {}

    public CharacterSave(string id, string fname, string lname, int sex, int yearsOld, int monthsOld, 
    string country, string state, List<LifeEventSave> dates, List<LifeEventSave> events)
    {
        FirstName = fname;
        LastName = lname;
        Sex = sex;
        YearsOld = yearsOld;
        MonthsOld = monthsOld;
        CountryName = country;
        State = state;
        DateLog = dates;
        EventLog = events;
    }

    public CharacterDetails LoadCharacter(CountryDatabase countryDb)
    {
        CharacterDetails details = new CharacterDetails();
        details.SetName(FirstName, LastName);
        details.SetSex((ESex)Sex);
        details.SetAge(MonthsOld, YearsOld);
        details.SetLocation(countryDb.GetCountryFromName(CountryName), State);

        List<LifeEventLog> dateLog = new List<LifeEventLog>();
        List<LifeEventLog> eventLog = new List<LifeEventLog>();

        foreach(var date in DateLog)
            dateLog.Add(date.LoadLifeEvent());

        foreach(var e in EventLog)
            eventLog.Add(e.LoadLifeEvent());


        details.SetLifeEvents(dateLog, eventLog);
        details.SetID(CharacterID);

        return details;
    }
}