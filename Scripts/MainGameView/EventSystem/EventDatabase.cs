using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public partial class EventDatabase : Node
{
    const string FILE_LOCATION = "res://Database/Events.json";              // Reference to the events database
    private List<LifeEventLog> _EventData = new List<LifeEventLog>();           // List of all events that can occur
    private List<EventAction> _ActionEventsData = new List<EventAction>();      // List of all the actions that attach to event

    public override void _Ready()
    {
        base._Ready();
        LoadEvents();
    }

    /// <summary>
    ///  Loads all the events from the database
    /// </summary>
    private void LoadEvents()
    {
        // Open the file and validate it is open
        var file = Godot.FileAccess.Open(FILE_LOCATION, Godot.FileAccess.ModeFlags.Read);
        if(file != null && file.IsOpen())
        {
            string text = file.GetAsText();             // Get the JSON data as a string
            var objects = JArray.Parse(text);           // Parse the JSON data into a JArray

            // Loop through each JSON object
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
            // Error Handling
            // TODO: Indicate there is a failure
            GD.Print($"Failed to load JSON File: Events Data");
        }

        
    }

    /// <summary>
    /// Creates a new random event
    /// </summary>
    /// <param name="age">Age category of the character</param>
    /// <returns>A new life event</returns>
    public LifeEventLog GetRandomEvent(EAgeCategory age)
    {
        // Create a random number generator
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();

        int loop = 0;               // Loop count to prevent infinite loop
        while(true)
        {
            // Select a random event & validate it
            LifeEventLog randomEvent = _EventData[rand.RandiRange(0, _EventData.Count - 1)].Copy();
            if(randomEvent != null)
            {
                // Check if the age category matches
                if(age == randomEvent.AgeCategory)
                {
                    // Get reference to the game controller & validate it
                    GameController game = GetNode<GameController>("/root/GameController");
                    if(game != null)
                    {
                        CharacterDetails character = game.CurrentCharacter;             // Get reference to the current character the player is playing as.
                        if(character != null)
                        {
                            // Check that the event has happened already. if so pass on to the next event
                            List<LifeEventLog> events = character.GetEventsFromDate(character.YearsOld, character.MonthsOld);
                            foreach(var e in events)
                            {
                                if(e.Text == randomEvent.Text)
                                    continue;
                            }

                            // Create the event and return it.
                            randomEvent.Year = character.YearsOld;
                            randomEvent.Month = character.MonthsOld;
                            randomEvent.ID = game.CurrentEventID;
                            return randomEvent;
                        }
                    }
                }
            }

            loop++;
            if(loop > 1000)
                break;
        }

        return null;
    }




    /// <summary>
    /// Determines the age category from the string
    /// </summary>
    /// <param name="category">Category as a string</param>
    /// <returns></returns>
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

    /// <summary>
    /// Determines sex from string
    /// </summary>
    /// <param name="sex">Sex as a string</param>
    /// <returns></returns>
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


    /// <summary>
    /// Determines event type from a string
    /// </summary>
    /// <param name="e">Event as a string</param>
    /// <returns></returns>
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

    /// <summary>
    /// Finds the relevant action from the name of the event
    /// </summary>
    /// <param name="name">Name of the event</param>
    /// <returns></returns>
    public EventAction GetActionEvent(string name)
    {
        foreach(var action in _ActionEventsData)
        {
            if(name == action)
                return action;
        }

        return null;
    }

    /// <summary>
    /// Selects a random birth event 
    /// </summary>
    /// <returns></returns>
    public LifeEventLog GetBirthEvent()
    {
        // Pre-define a list of birth events
        List<LifeEventLog> birthEvents = new List<LifeEventLog>();

        // Loops through each event
        foreach(var e in _EventData)
        {
            // Validate the event, and check that it is a birth event
            // Then add the event to the pre-defined list
            if(e != null && e.Type == ELifeEventType.BIRTH)
            {
                birthEvents.Add(e);
            }
        }

        // Create a random number generator
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();

        LifeEventLog selectedEvent = null;              // Pre-define the selected event

        selectedEvent = birthEvents[rand.RandiRange(0, birthEvents.Count - 1)].Copy();         // Get a random event
        // Validate the event and update its properties
        if(selectedEvent != null)
        {
            selectedEvent.IsBornEvent = true;
            selectedEvent.Year = 0;
            selectedEvent.Month = 0;
        }
        

        return selectedEvent;               // Return the created event
    }


}

public class EventData
{
    public string EventText;
    public string AgeCategory;
    public string EventType;
    public string Sex;
}