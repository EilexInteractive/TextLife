using Godot;
using System;
using System.Collections.Generic;

public enum ESex
{
    MALE,
    FEMALE,
    ALL
}

public enum EAgeCategory
{
    BIRTH,
    BABY,
    TODDLER,
    CHILD,
    TEEN,
    YOUNG_ADULT,
    OLD_ADULT,
    ELDERLY,
    ALL
}

public class CharacterDetails
{
    private string _CharacterID;
    public string CharacterID;
    private string _FirstName;
    public string FirstName { get => _FirstName;  }
    private string _LastName;
    public string LastName { get => _LastName; }

    private ESex _Sex;
    public ESex Sex { get => _Sex; }
    private int _YearsOld;
    public int YearsOld { get => _YearsOld; }
    private int _MonthsOld;
    public int MonthsOld { get => _MonthsOld; }

    

    private Country _Country;
    public Country Country { get => _Country; }
    private string _State;
    public string State { get => _State; }

    private int _Money;
    public int Money { get => _Money; }

    private bool _IsAlive = true;
    public bool IsAlive { get => _IsAlive; }

    private CharacterStats _Stats;
    public CharacterStats Stats { get => _Stats; }

    private List<LifeEventLog> _EventDates = new List<LifeEventLog>();
    public List<LifeEventLog> EventDates { get => _EventDates; }
    private List<LifeEventLog> _LifeEventLog = new List<LifeEventLog>();
    public List<LifeEventLog> LifeEventLog { get => _LifeEventLog; }

    private EAgeCategory _CharacterAgeCategory;
    public EAgeCategory CharacterAgeCategory { get => _CharacterAgeCategory; }

    // === Relationship System === //
    private List<Relationship> _Relationships = new List<Relationship>();
    public List<Relationship> Relationships { get => _Relationships; }

    // === Pending relationship events === //
    private List<LifeEventRequest> _PendingRelationshipEvents = new List<LifeEventRequest>();
    public List<LifeEventRequest> PendingRelationshipEvents { get => _PendingRelationshipEvents; }

    private List<LifeEventRequest> _AcceptedRelationshipEvents = new List<LifeEventRequest>();
    

    public CharacterDetails(string firstName, string lastName, int years, int months, ESex gender, Country country, string state, int money, GameController game)
    {
        _FirstName = firstName;
        _LastName = lastName;
        _Sex = gender;
        _YearsOld = years;
        _MonthsOld = months;
        _Country = country;
        _State = state;
        _Money = money;
    }

    public CharacterDetails()
    {}


    public void SetCharacter(string firstName, string lastName, int years, int months, ESex gender, Country country, string state, int money, GameController game = null)
    {
        _FirstName = firstName;
        _LastName = lastName;
        _Sex = gender;
        _YearsOld = years;
        _MonthsOld = months;
        _Country = country;
        _State = state;
        _Money = money;
    }
    

    public void SetCharacterStats(CharacterStats stats) => _Stats = stats;

    public string GetAgeAsString() => $"{_YearsOld} Years Old, {_MonthsOld} Months";

    /// <summary>
    /// Sets the age of the character
    /// </summary>
    /// <param name="months">How many months old</param>
    /// <param name="years">How many years old</param>
    public void SetAge(int months, int years)
    {
        _MonthsOld = months;
        _YearsOld = years;
    }

    /// <summary>
    /// Adds a new life event to the list
    /// </summary>
    /// <param name="lifeEvent">Life event to add</param>

    public void AddLifeEvent(LifeEventLog lifeEvent, bool dispatchEvent = true, WorldEventData worldEvent = null)
    {
        if(lifeEvent.Type == ELifeEventType.DATE)
        {
            _EventDates.Add(lifeEvent);
        } else 
        {
            _LifeEventLog.Add(lifeEvent);
            if(dispatchEvent)
            {
                switch(lifeEvent.Type)
                {
                    case ELifeEventType.EVENT:
                        lifeEvent.Dispatch();
                        break;
                    case ELifeEventType.BIRTH:
                        lifeEvent.Dispatch();
                        break;
                    case ELifeEventType.WORLD_WAR_START:
                    case ELifeEventType.WORLD_WAR_UPDATE:
                    case ELifeEventType.WORLD_WAR_END:
                        lifeEvent.Dispatch(lifeEvent.Type, worldEvent);
                        break;
                    
                }
            }
        }
    }

    /// <summary>
    /// Increase the age of the character by one month
    /// </summary>
    /// <param name="game">Reference to the game controller</param>
    public void AgeUp(GameController game)
    {
        if(game == null)
            return;

        _MonthsOld += 1;
        if(_MonthsOld > 11)
        {
            _MonthsOld = 0;
            _YearsOld += 1;
        }

        _CharacterAgeCategory = GetAgeCategory();


        AddLifeEvent(CreateDateLog(game));
    }

    /// <summary>
    /// Creates a new date event
    /// </summary>
    /// <param name="game">Reference to the game controller</param>
    /// <returns>LIfe event in the form of a date</returns>
    private LifeEventLog CreateDateLog(GameController game)
    {
        string monthAsString = game.GetMonthAsString();
        LifeEventLog dateLog = new LifeEventLog($"{monthAsString} - {_YearsOld} Years, {_MonthsOld} Months", ELifeEventType.DATE, _CharacterAgeCategory, _Sex);
        dateLog.Year = _YearsOld;
        dateLog.Month = _MonthsOld;
        return dateLog.Copy(game.CurrentEventID);
    }

    /// <summary>
    /// Accepts the passed in event and performs any necesarry actions
    /// </summary>
    /// <param name="e">Event to accept</param>
    /// <param name="eventIndex">index id for the event</param>
    public void AcceptLifeEvent(LifeEventRequest e, int eventIndex)
    {
        // Ensure the event is in the pending relationship events
        if(_PendingRelationshipEvents.Contains(e))
        {
            _AcceptedRelationshipEvents.Add(e);             // Add the event to the accepted events
            _PendingRelationshipEvents.Remove(e);           // Remove from the pending events

            LifeEventLog eventLog = e.CorrespondingEvent.Copy();        // Create a copy of the event
            eventLog.ID = eventIndex;           // Assign the event ID

            _Stats.ReduceTiredness(e.CorrespondingEvent.Tiredness);     // Reduce tiredness meter of the character          

            _LifeEventLog.Add(eventLog);                // Add to the event log
        }
    }

    public void DeclineLifeEvent(LifeEventRequest e)
    {
        if(_PendingRelationshipEvents.Contains(e))
        {
            _PendingRelationshipEvents.Remove(e);
        }
    }

    public void AddEventRequest(LifeEventRequest request) 
    {
        _PendingRelationshipEvents.Add(request);
    }


    /// <summary>
    /// Determines the age category of the character
    /// </summary>
    /// <returns>The age category</returns>
    public EAgeCategory GetAgeCategory()
    {
        if(_YearsOld < 3)
        {
            return EAgeCategory.BABY;
        } else if(_YearsOld < 5)
        {
            return EAgeCategory.TODDLER;
        } else if(_YearsOld < 13)
        {
            return EAgeCategory.CHILD;
        } else if(_YearsOld < 20)
        {
            return EAgeCategory.TEEN;
        } else if(_YearsOld < 35)
        {
            return EAgeCategory.YOUNG_ADULT;
        } else if(_YearsOld < 65)
        {
            return EAgeCategory.OLD_ADULT;
        } else 
        {
            return EAgeCategory.ELDERLY;
        }
    }

    /// <summary>
    /// Checks if the character has a born event
    /// </summary>
    /// <returns>If there is a born event</returns>
    private bool ContainsBornEvent()
    {
        if(_LifeEventLog.Count > 0)
        {
            foreach(var log in _LifeEventLog)
            {
                if(log.IsBornEvent)
                    return true;
            }
        }

        return false;
    }

    public void AddRelationship(Relationship r) 
    {
        if(r != null)
        {
            _Relationships.Add(r);
        }
    }

    public void UpdateRelationships()
    {
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();
        foreach(var relationship in _Relationships)
        {
            relationship.ReduceRelationship();
        }
    }

    public void ClearAllRequest()
    {
        _PendingRelationshipEvents.Clear();
    }

    public void SetCharacterName(string[] name)
    {
        _FirstName = name[0];
        _LastName = name[1];
    }

    public void SetLocation(Country country, string state)
    {
        _Country = country;
        _State = state;
    }

    public void SetSex(ESex sex) => _Sex = sex;
    public void SetName(string firstName, string lastName)
    {
        _FirstName = firstName;
        _LastName = lastName;
    }

    public void SetLifeEvents(List<LifeEventLog> dates, List<LifeEventLog> events)
    {
        _EventDates = dates;
        _LifeEventLog = events;
    }

    public bool HasRelationship(string id_1, string id_2)
    {
        foreach(var relationship in _Relationships)
        {
            if(id_1 == relationship.Character_1._CharacterID && id_2 == relationship.Character_2.CharacterID)
            {
                return true;
            } else if(id_1 == relationship.Character_2.CharacterID && id_2 == relationship.Character_1.CharacterID)
            {
                return true;
            }
        }

        return false;
        
    }

    public void GetParentNames(out string parentA, out string parentB)
    {
        parentA = "";
        parentB = "";
        foreach(var relation in _Relationships)
        {
            if(relation.RelationshipType == ERelationshipType.PARENT)
            {
                if(relation.Character_1 == this)
                {
                    if(parentA == "")
                    {
                        parentA = relation.Character_2.FirstName + " " + relation.Character_2.LastName;
                    } else 
                    {
                        parentB = relation.Character_2.FirstName + " " + relation.Character_2.LastName;
                    }
                } else 
                {
                    if(parentA == "")
                    {
                        parentA = relation.Character_1.FirstName + " " + relation.Character_1.LastName;
                    } else 
                    {
                        parentB = relation.Character_1.FirstName + " " + relation.Character_1.LastName;
                    }
                }
            }
        }
            
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Money in the form of a string</returns>
    public string GetMoneyAsString()
    {
        string moneyStr = "$";

        string currentMoney = _Money.ToString();
        
        switch(currentMoney.Length)
        {
            case 4:
                int[] thousand = {0};
                moneyStr += CreateMoneyString(thousand, currentMoney);
                break;
            case 5:
                int[] tenThousand = { 1 };
                moneyStr += CreateMoneyString(tenThousand, currentMoney);
                break;
            case 6:
                int[] hundredThousand = { 2 };
                moneyStr += CreateMoneyString(hundredThousand, currentMoney);
                break;
            case 7:
                int[] million = { 0, 3};
                moneyStr += CreateMoneyString(million, currentMoney);
                break;
            case 8:
                int[] billion = {0, 3, 6};
                moneyStr += CreateMoneyString(billion, currentMoney);
                break;
            case 9:
                int[] trillion = {0, 3, 6, 9};
                moneyStr += CreateMoneyString(trillion, currentMoney);
                break;
            default:
                int[] def = { 0 };
                moneyStr += CreateMoneyString(def, currentMoney);
                break;
        }   

        return moneyStr;
    }

    private string CreateMoneyString(int[] commaIndex, string money)
    {
        string str = "";
        for(int i = 0; i < money.Length; ++i)
        {
            str += money[i];

            if(money.Length > 3)
            {
                for(int j = 0; j < money.Length; ++j)
            {
                if(i == commaIndex[j])
                    str += ",";
            }
            }
        }

        return str;
    }

    public CharacterSave CreateCharacterSave()
    {
        int gender = (int)_Sex;
        List<LifeEventSave> dateSaves = new List<LifeEventSave>();
        List<LifeEventSave> eventLog = new List<LifeEventSave>();
        
        foreach(var date in _EventDates)
            dateSaves.Add(date.CreateEventSave());
        
        foreach(var e in _LifeEventLog)
            eventLog.Add(e.CreateEventSave());


        return new CharacterSave(_CharacterID, _FirstName, _LastName, gender, _YearsOld, _MonthsOld, _Country.Name, _State, dateSaves, eventLog);
    }

    public List<LifeEventLog> GetEventsFromDate(int year, int month)
    {
        List<LifeEventLog> logs = new List<LifeEventLog>();
        foreach(var log in _LifeEventLog)
        {
            if(log.Year == year && log.Month == month)
                logs.Add(log);
        }

        return logs;
    }

    public void SetID(string id) => _CharacterID = id;

    public override string ToString()
    {
        return FirstName + " " + LastName;
    }
}