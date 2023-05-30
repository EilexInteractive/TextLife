using Godot;
using System.Collections.Generic;
using System;

public partial class WorldController : Node
{

    private GameController _Game;

    private List<CharacterDetails> _InWorldCharacters = new List<CharacterDetails>();
    public List<CharacterDetails> InWorldCharacters { get => _InWorldCharacters; }

    private const float RANDOM_EVENT_CHANCE = 0.7f;                 // Chance that a specific character will receive an event

    // === World Events === //
    private List<WorldEventData> _CurrentWorldEvents = new List<WorldEventData>();
    private List<WorldEventData> _ClosedWorldEvents = new List<WorldEventData>();
    private const float RANDOM_WORLD_EVENT_CHANCE = 0.9f;                   // Chance that there will be an event update
    private const float NEW_WORLD_EVENT_CHANCE = 0.1f;                      // Chance that there will be a new world event

    // === Relationship Events === //
    private const float CHANCE_OF_RELATIONSHIP_EVENT = 0.1f;

    public override void _Ready()
    {
        base._Ready();
        _Game = GetNode<GameController>("/root/GameController");
    }

    public void Advance()
    {
        AgeUpAllCharacters();
        GenerateCharacterEvents();
        GenerateRelationshipEvents();
        GenerateWorldEvents();
    }

    /// <summary>
    /// Adds an event to everyones event log
    /// </summary>
    /// <param name="log">Life event that is being added</param>
    public void AddEventToAllLogs(LifeEventLog log)
    {
        foreach(var character in _InWorldCharacters)
            character.AddLifeEvent(log);
    }

    /// <summary>
    /// Generates a random event for an individual character
    /// </summary>
    public void GenerateCharacterEvents()
    private void GenerateCharacterEvents()
    {
        // Create a random number generator
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();

        // Get reference to the event database, if it's not validate then don't proceed
        EventDatabase eventDb = GetNode<EventDatabase>("/root/EventDatabase");
        if(eventDb == null)
            return;

        // Loop through each character in the world
        foreach(var character in _InWorldCharacters)
        {
            float hasRandomEvent = rand.Randf();                    // Determine a random number between 0 and 1
            // Check if the random number equals a world event
            if(hasRandomEvent > RANDOM_EVENT_CHANCE)
            {
                // Get a random event for the character
                LifeEventLog lifeEvent = eventDb.GetRandomEvent(character.CharacterAgeCategory);
                if(lifeEvent != null)
                {
                    // Determine if this is for the current character 
                    // If it is than dispatch the event for the character
                    // otherwise skip the event
                    // REFACTOR: Change to check if the event is for the player character
                    if(character == _Game.CurrentCharacter)
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

    /// <summary>
    /// Generates a random event from the world
    /// </summary>
    public void GenerateWorldEvents()
    {
        // Get reference to the event database and validate it
        EventDatabase eventDb = GetNode<EventDatabase>("/root/EventDatabase");
        if(eventDb == null)
            return;

        // Create random number generator
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();

        float chance = rand.Randf();                // Generate a random value between 0 and 1, used to determine if we should create an event
        if(chance > RANDOM_WORLD_EVENT_CHANCE)
        {
            rand.Randomize();

            // Check if there are any events already in the world
            // if there isn't then generate a new one
            if(_CurrentWorldEvents.Count > 0)
            {
                // Determine whether to create a new world event or add to an existing one
                if(rand.Randf() > NEW_WORLD_EVENT_CHANCE)
                {
                    WorldEventData worldEventData = eventDb.CreateNewEvent(_Game);
                    _CurrentWorldEvents.Add(worldEventData);
                } else 
                {
                    // Get a reference to a random event that has already taken place and is eligable
                    WorldEventData existingWorldEventData = _CurrentWorldEvents[rand.RandiRange(0, _CurrentWorldEvents.Count - 1)];
                    if(existingWorldEventData != null)
                    {
                        int loopCount = 0;              // Define a loop count to prevent infinite loop
                        while(true)
                        {
                            // Get a random event from the event database
                            LifeEventLog newEvent = eventDb.GetRandomEvent(existingWorldEventData.GetEventType());
                            // Make sure this event hasn't already ocurred
                            if(!existingWorldEventData.hasEventOcurred(newEvent))
                            {
                                existingWorldEventData.AddEvent(newEvent);
                            }

                            // Loop count check
                            loopCount++;
                            if(loopCount > 1000)
                                break;
                        }
                        
                    }
                }
            } else 
            {
                 WorldEventData worldEventData = eventDb.CreateNewEvent(_Game);
                _CurrentWorldEvents.Add(worldEventData);
            }
        }
    }

    /// <summary>
    /// Increase the age of the character
    /// </summary>
    public void AgeUpAllCharacters()
    {
        foreach(var character in _InWorldCharacters)
        {
            character.AgeUp(_Game);
        }
    }

    /// <summary>
    /// Adds a new character to the world
    /// </summary>
    /// <param name="character">Character to add to the world</param>
    public void AddCharacterToWorld(CharacterDetails character) 
    {
        if(!_InWorldCharacters.Contains(character))
            _InWorldCharacters.Add(character);
    }

    /// <summary>
    /// Gets the corresponding character from the ID
    /// </summary>
    /// <param name="characterID">ID of the character to get</param>
    /// <returns>Character</returns>
    public CharacterDetails GetCharacterFromWorld(string characterID)
    {
        foreach(var character in _InWorldCharacters)
        {
            if(character.CharacterID == characterID)
                return character;
        }

        return null;
    }

    /// <summary>
    /// Adds all the world events into save format
    /// </summary>
    /// <returns></returns>
    public List<WorldEventSave> GetWorldEventsAsSave()
    {
        List<WorldEventSave> saveList = new List<WorldEventSave>();             // Pre-define the list of world events
        // Loop through each world event
        foreach(var e in _CurrentWorldEvents)
        {
            WorldEventSave save = new WorldEventSave();             // Create the save
            // Set the save details
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

    /// <summary>
    /// Updates all the relationships
    /// </summary>
    public void UpdateAllRelationships()
    {
        foreach(var character in _InWorldCharacters)
        {
            character.UpdateRelationships();
        }
    }
}