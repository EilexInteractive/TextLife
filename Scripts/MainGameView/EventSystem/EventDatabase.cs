using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public enum EEventActionType
{
    NULL,
    STRING,
    INT,
    FLOAT,
    BOOL,
    WORLD_EVENT
}

public partial class EventDatabase : Node
{
    const string FILE_LOCATION = "res://Database/Events.json";              // Reference to the events database
    private List<LifeEventLog> _EventData = new List<LifeEventLog>();           // List of all events that can occur
    private List<LifeEventLog> _WorldEventData = new List<LifeEventLog>();                  // Reference to world events we can pull from
    private List<LifeEventLog> _RelationshipEvents = new List<LifeEventLog>();              // Reference to all the relationship events


    private List<EventAction> _ActionEventsData = new List<EventAction>();      // List of all the actions that attach to event
    private List<EventAction> _ActionEventsString = new List<EventAction>();
    private List<EventAction> _ActionEventsBool = new List<EventAction>();
    private List<EventAction> _ActionEventsInt = new List<EventAction>();
    private List<EventAction> _ActionEventsFloat = new List<EventAction>();
    private List<EventAction> _ActionWorldEvent = new List<EventAction>();

    private int[] _WorldEventsIndex = { 3, 4, 5};

    public override void _Ready()
    {
        base._Ready();
         AddAction("TestFunction", TestEvent);
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

                List<EventAction> parsedEvents = ParseEvents(data.Events);

                
                // Create the event log
                LifeEventLog log = new LifeEventLog(dataText, dataEventType, dataCategory, dataSex);
                if(log != null)
                {
                    foreach(var e in parsedEvents)
                    {
                        if(e != null)
                            log.Actions.Add(e);
                        else 
                            GD.Print("Error Occured: Failed to load event data");
                    }
                } else 
                {
                    GD.Print("Error Occured: Failed to create event");
                }

                switch(dataEventType)
                {
                    case ELifeEventType.WORLD_WAR_START:
                    case ELifeEventType.WORLD_WAR_UPDATE:
                    case ELifeEventType.WORLD_WAR_END:
                        _WorldEventData.Add(log);
                        break;
                    case ELifeEventType.RELATIONSHIP_EVENT:
                        _RelationshipEvents.Add(log);
                        break;
                    default:
                        _EventData.Add(log);
                        break;
                }
            }

            file.Close();
        } else 
        {
            // Error Handling
            // TODO: Indicate there is a failure
            GD.Print($"Failed to load JSON File: Events Data");
        }

        
    }

    public void TestEvent()
    {
        GD.Print("Hello World");
    }

    public EventAction GetEventFromName(string name)
    {
        foreach(var e in _ActionEventsData)
            if(e.EventName == name)
                return e;

        foreach(var e in _ActionEventsBool)
            if(e.EventName == name)
                return e;

        foreach(var e in _ActionEventsFloat)
            if(e.EventName == name)
                return e;

        foreach(var e in _ActionEventsInt)
            if(e.EventName == name)
                return e;

        foreach(var e in _ActionEventsString)
            if(e.EventName == name)
                return e;

        foreach(var e in _ActionWorldEvent)
            if(e.EventName == name)
                return e;

        return null;
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

    public LifeEventLog GetRandomEvent(ELifeEventType type)
    {
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();
        List<LifeEventLog> events = new List<LifeEventLog>();

        foreach(var e in _WorldEventData)
        {
            if(e.Type == type)
            {
                events.Add(e);
            }
        }

        return events[rand.RandiRange(0, events.Count - 1)];
    }

    public LifeEventLog GetWorldEvent(List<WorldEventData> currentEvents)
    {
        WorldController world = GetNode<WorldController>("/root/WorldController");
        if(_WorldEventData.Count > 0)
        {
            RandomNumberGenerator rand = new RandomNumberGenerator();
            int loopCount = 0;
            while(true)
            {
                int randValue = rand.RandiRange(0, 6);
                for(int i = 0; i < _WorldEventsIndex.Length; ++i)
                {
                    if(_WorldEventsIndex[i] == randValue)
                    {
                        switch((ELifeEventType)randValue)
                        {
                            case ELifeEventType.WORLD_WAR_START:
                                return GetRandomEventOfType((ELifeEventType)randValue);
                            case ELifeEventType.WORLD_WAR_UPDATE:
                                foreach(var cEvent in currentEvents)
                                    if(cEvent.GetEventType() == (ELifeEventType)randValue)
                                        return GetRandomEventOfType(ELifeEventType.WORLD_WAR_UPDATE);
                                break;
                            case ELifeEventType.WORLD_WAR_END:
                                foreach(var cEvent in currentEvents)
                                    if(cEvent.GetEventType() == (ELifeEventType)randValue)
                                        return GetRandomEventOfType(ELifeEventType.WORLD_WAR_END);
                                break;
                            default:
                                break;
                        }
                        
                    } else 
                    {
                        break;
                    }
                }
            }
        }

        return null;
    }

    public LifeEventLog GetRandomEventOfType(ELifeEventType type)
    {
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();

        int loopCount = 0;
        while(true)
        {

            LifeEventLog e =_WorldEventData[rand.RandiRange(0, _WorldEventData.Count - 1)];
            if(e.Type == type)
                return e;

            loopCount++;
            if(loopCount > 1000)
                break;
        }

        return null;
    }

    private bool HasWorldWar()
    {
        foreach(var e in _WorldEventData)
        {
            if(e.Type == ELifeEventType.WORLD_WAR_START)
                return true;
        }

        return false;
    }

    public WorldEventData CreateNewEvent(GameController game)
    {
        // Get reference to the required controllers & validate them
        WorldController world = GetNode<WorldController>("/root/WorldController");
        WorldEventMethods eventMethods = GetNode<WorldEventMethods>("/root/EventMethods");
        if(game == null || world == null || eventMethods == null)
            return null;

        // Check that we have loaded the world data
        if(_WorldEventData.Count > 0)
        {
            LifeEventLog log = GetWorldEvent(world.CurrentWorldEvents).Copy();         // Get a random world event
            
            if(log != null)
            {
                log.ID = game.CurrentEventID;               // Set the ID of the current event
                WorldEventData newWorldEventData = null;            // Predefine the world event

                // Get reference to the country database
                CountryDatabase countryDb = GetNode<CountryDatabase>("/root/CountryDatabase");
                // Select the country for the origin of the world event
                Country origin = countryDb.GetRandomCountry();

                if(log.Type == ELifeEventType.WORLD_WAR_START)
                {
                    newWorldEventData = new WorldEventData(origin, countryDb.GetRandomCountry());
                    newWorldEventData.AddEvent(log, eventMethods);
                    log.Dispatch();
                    newWorldEventData.SetEventType(log.Type);
                    return newWorldEventData;
                }
                
            }
            
        }
        
        return null;
    }

    private List<EventAction> ParseEvents(string eventsText)
    {
        List<EventAction> _actions = new List<EventAction>();
        string[] actionsString = eventsText.Split(",");

        foreach(var e in actionsString)
        {
            EventAction action = GetEventFromName(e);
            if(action != null)
            {
                _actions.Add(action);
            }
        }

        return _actions;
    }

    public void AddAction(string eventName, Action e) 
    {
        EventAction action = new EventAction();
        action.EventName = eventName;
        action.EventActionNoProps += e;
        action.EventType = EEventActionType.NULL;
        _ActionEventsData.Add(action);
    }

    public void AddAction(string eventName, Action<ELifeEventType, WorldEventData, LifeEventLog> e)
    {
        EventAction action = new EventAction();
        action.EventName = eventName;
        action.EventActionWorldEvent += e;
        action.EventType = EEventActionType.WORLD_EVENT;
        _ActionWorldEvent.Add(action);
    }

    public void AddAction<T>(string eventName, Action<T> e, EEventActionType type)
    {
        EventAction creatingEvent = new EventAction();
        creatingEvent.EventName = eventName;
        switch(type)
        {
            case EEventActionType.STRING:
                creatingEvent.EventActionString += e as Action<string>;
                creatingEvent.EventType = EEventActionType.STRING;
                _ActionEventsString.Add(creatingEvent);
                break;
            case EEventActionType.INT:
                creatingEvent.EventActionInt += e as Action<int>;
                creatingEvent.EventType = EEventActionType.INT;
                _ActionEventsInt.Add(creatingEvent);
                break;
            case EEventActionType.FLOAT:
                creatingEvent.EventActionFloat += e as Action<float>;
                creatingEvent.EventType = EEventActionType.FLOAT;
                _ActionEventsFloat.Add(creatingEvent);
                break;
            case EEventActionType.BOOL:
                creatingEvent.EventActionBool += e as Action<bool>;
                creatingEvent.EventType = EEventActionType.BOOL;
                _ActionEventsBool.Add(creatingEvent);
                break;
        }
    }

    /// <summary>
    /// Gets a random relationship event
    /// </summary>
    /// <param name="to">Who the request is for</param>
    /// <param name="from">Who the request is from</param>
    /// <returns></returns>
    public LifeEventRequest GetRelationshipEvent(CharacterDetails to, CharacterDetails from)
    {
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();
        LifeEventLog lifeEvent = _RelationshipEvents[rand.RandiRange(0, _RelationshipEvents.Count - 1)];
        if(lifeEvent != null)
        {
            return new LifeEventRequest(lifeEvent, from, to);
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
            case "WorldWarStart":
                return ELifeEventType.WORLD_WAR_START;
            case "WorldWarUpdate":
                return ELifeEventType.WORLD_WAR_UPDATE;
            case "WorldWarEnd":
                return ELifeEventType.WORLD_WAR_END;
            case "RelationshipEvent":
                return ELifeEventType.RELATIONSHIP_EVENT;
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
    [JsonProperty]
    public string EventText;
    [JsonProperty]
    public string AgeCategory;
    [JsonProperty]
    public string EventType;
    [JsonProperty]
    public string Sex;
    [JsonProperty]
    public string Events;
}