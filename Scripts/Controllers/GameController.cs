using Godot;
using System;
using System.Collections.Generic;
public partial class GameController : Node
{
    // Reference to where we store the application data
    public string ApplicationFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Eilex Interactive/TextLife";
    public PlayerPrefs PlayerPrefs;                         // Reference to the player prefs
	public CharacterDetails CurrentCharacter;               // reference to the current character

    
    // === Current Date Data
    private int _Month = 1;
    public int CurrentMonth { get => _Month; }
    
    public int CurrentEventID = 0;

    public string ReturnTo = "";                    // Reference to the scene we need to return to.


    // === Characters in world data === //
    private List<CharacterDetails> _InWorldCharacters = new List<CharacterDetails>();
    public List<CharacterDetails> InWorldCharacters { get => _InWorldCharacters; }
    private const float RANDOM_EVENT_CHANCE = 0.7f;


    // === World Events === //
    private List<WorldEventData> _CurrentWorldEvents = new List<WorldEventData>();
    private List<WorldEventData> _ClosedWorldEvents = new List<WorldEventData>();
    private const float RANDOM_WORLD_EVENT_CHANCE = 0.9f;
    private const float NEW_WORLD_EVENT_CHANCE = 0.6f;
    

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public void IncrementMonth()
    {
        _Month += 1;
        if(_Month > 11)
        {
            _Month = 0;
        }

        CurrentEventID += 1;
    }

    public string GetMonthAsString()
    {
        switch(_Month)
        {
            case 0:
                return "January";
            case 1:
                return "February";
            case 2:
                return "March";
            case 3:
                return "April";
            case 4:
                return "May";
            case 5:
                return "June";
            case 6:
                return "July";
            case 7:
                return "August";
            case 8:
                return "September";
            case 9:
                return "October";
            case 10:
                return "November";
            case 11:
                return "December";
            default:
                return "#ERROR#";
        }
        

    }

    public void AddEventToAllLogs(LifeEventLog log)
    {
        foreach(var character in _InWorldCharacters)
            character.AddLifeEvent(log);
    }

    public void GenerateCharacterEvents()
    {
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();

        EventDatabase eventDb = GetNode<EventDatabase>("/root/EventDatabase");
        if(eventDb != null)
            return;

        foreach(var character in _InWorldCharacters)
        {
            float hasRandomEvent = rand.Randf();
            if(hasRandomEvent > RANDOM_EVENT_CHANCE)
            {
                LifeEventLog lifeEvent = eventDb.GetRandomEvent(character.CharacterAgeCategory);
                if(lifeEvent != null)
                {
                    if(character == CurrentCharacter)
                    {
                        character.AddLifeEvent(lifeEvent, true);
                    } else 
                    {
                         character.AddLifeEvent(lifeEvent, false);
                    }
                }
            }
        }
    }

    public void GenerateWorldEvents()
    {
        EventDatabase eventDb = GetNode<EventDatabase>("/root/EventDatabase");
        if(eventDb == null)
            return;

        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();

        float chance = rand.Randf();
        if(chance > RANDOM_WORLD_EVENT_CHANCE)
        {
            rand.Randomize();
            if(_CurrentWorldEvents.Count > 0)
            {
                if(rand.Randf() > NEW_WORLD_EVENT_CHANCE)
                {
                    WorldEventData worldEventData = eventDb.CreateNewEvent(this);
                    _CurrentWorldEvents.Add(worldEventData);
                } else 
                {
                    WorldEventData existingWorldEventData = _CurrentWorldEvents[rand.RandiRange(0, _CurrentWorldEvents.Count - 1)];
                    if(existingWorldEventData != null)
                    {
                        int loopCount = 0;
                        while(true)
                        {

                            LifeEventLog newEvent = eventDb.GetRandomEvent(existingWorldEventData.GetEventType());
                            if(!existingWorldEventData.hasEventOcurred(newEvent))
                            {
                                existingWorldEventData.AddEvent(newEvent);
                            }

                            loopCount++;
                            if(loopCount > 1000)
                                break;
                        }
                        
                    }
                }
            } else 
            {
                 WorldEventData worldEventData = eventDb.CreateNewEvent(this);
                _CurrentWorldEvents.Add(worldEventData);
            }
        }
    }

    public void AgeUpAllCharacters()
    {
        foreach(var character in _InWorldCharacters)
        {
            character.AgeUp(this);
        }
    }

    public void AddCharacterToWorld(CharacterDetails character) 
    {
        if(!_InWorldCharacters.Contains(character))
            _InWorldCharacters.Add(character);
    }

    public CharacterDetails GetCharacterFromWorld(string characterID)
    {
        foreach(var character in _InWorldCharacters)
        {
            if(character.CharacterID == characterID)
                return character;
        }

        return null;
    }

    public List<WorldEventSave> GetWorldEventsAsSave()
    {
        List<WorldEventSave> saveList = new List<WorldEventSave>();
        foreach(var e in _CurrentWorldEvents)
        {
            WorldEventSave save = new WorldEventSave();
            save.IsOpen = true;
            save.OriginCountryName = e.CountryOrigin.Name;
            save.InvolvedCountryName = e.InvolvedCountry.Name;
            foreach(var lifeEvent in e.EventsOcurred)
            {
                save.Events.Add(lifeEvent.CreateEventSave());
            }
        }

        return saveList;
    }

    public void UpdateAllRelationships()
    {
        foreach(var character in _InWorldCharacters)
        {
            character.UpdateRelationships();
        }
    }
}
