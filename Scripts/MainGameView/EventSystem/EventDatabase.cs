using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public partial class EventDatabase : Node
{
    const string FILE_LOCATION = "res://Database/Events.json";
    private List<LifeEventLog> _EventData = new List<LifeEventLog>();

    public override void _Ready()
    {
        base._Ready();
        LoadEvents();
    }

    private void LoadEvents()
    {
        var file = Godot.FileAccess.Open(FILE_LOCATION, Godot.FileAccess.ModeFlags.Read);
        if(file != null && file.IsOpen())
        {
            string text = file.GetAsText();
            var objects = JArray.Parse(text);
            foreach(var obj in objects)
            {
                // Get json and deserialize it
                JToken token = obj;
                EventData data = JsonConvert.DeserializeObject<EventData>(token.ToString());
                // Create life event properties
                string dataText = data.EventText;
                EAgeCategory dataCategory = GetAgecategory(data.AgeCategory);
                ELifeEventType dataEventType = GetEventType(data.EventType);
                ESex dataSex = GetSexCategory(data.Sex);
                
                // Create the event log
                LifeEventLog log = new LifeEventLog(dataText, dataEventType, dataCategory, dataSex);
                _EventData.Add(log);
            }

            file.Close();
        } else 
        {
            GD.Print($"Failed to load JSON File: Events Data");
        }

        
    }



    private EAgeCategory GetAgecategory(string category)
    {
        switch(category)
        {
            case "BirthEvent":
                return EAgeCategory.BIRTH;
            case "Baby":
                return EAgeCategory.BABY;
            case "Toddler":
                return EAgeCategory.TODDLER;
            case "Child":
                return EAgeCategory.CHILD;
            case "Teen":
                return EAgeCategory.TEEN;
            case "YoungAdult":
                return EAgeCategory.YOUNG_ADULT;
            case "OldAdult":
                return EAgeCategory.OLD_ADULT;
            case "Elderly":
                return EAgeCategory.ELDERLY;
            default:
                return EAgeCategory.ALL;
        }
    }

    private ESex GetSexCategory(string sex)
    {
        switch(sex)
        {
            case "Male":
                return ESex.MALE;
            case "Female":
                return ESex.MALE;
            default:
                return ESex.ALL;
        }
    }

    private ELifeEventType GetEventType(string e)
    {
        switch(e)
        {
            case "Event":
                return ELifeEventType.EVENT;
            case "Birth":
                return ELifeEventType.BIRTH;
            case "Date":
                return ELifeEventType.DATE;
            default:
                return ELifeEventType.EVENT;
        }
    }

    public LifeEventLog GetBirthEvent()
    {
        List<LifeEventLog> birthEvents = new List<LifeEventLog>();
        foreach(var e in _EventData)
        {
            if(e != null && e.Type == ELifeEventType.BIRTH)
            {
                birthEvents.Add(e);
            }
        }

        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();

        return birthEvents[rand.RandiRange(0, birthEvents.Count - 1)];
    }


}

public class EventData
{
    public string EventText;
    public string AgeCategory;
    public string EventType;
    public string Sex;
}