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

    private CharacterStats _Stats;
    public CharacterStats Stats { get => _Stats; }

    private List<LifeEventLog> _EventDates = new List<LifeEventLog>();
    public List<LifeEventLog> EventDates { get => _EventDates; }
    private List<LifeEventLog> _LifeEventLog = new List<LifeEventLog>();
    public List<LifeEventLog> LifeEventLog { get => _LifeEventLog; }

    private EAgeCategory _CharacterAgeCategory;
    public EAgeCategory CharacterAgeCategory { get => _CharacterAgeCategory; }

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

    public void SetAge(int months, int years)
    {
        _MonthsOld = months;
        _YearsOld = years;
    }

    public void AddLifeEvent(LifeEventLog lifeEvent)
    {
        if(lifeEvent.Type == ELifeEventType.DATE)
        {
            _EventDates.Add(lifeEvent);
        } else 
        {
            _LifeEventLog.Add(lifeEvent);
        }
    }

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


        AddLifeEvent(CreateDateLog(game));
    }

    private LifeEventLog CreateDateLog(GameController game)
    {
        string monthAsString = game.GetMonthAsString();
        LifeEventLog dateLog = new LifeEventLog($"{monthAsString} - {_YearsOld} Years, {_MonthsOld} Months", ELifeEventType.DATE, _CharacterAgeCategory, _Sex);
        return dateLog.Copy(game.CurrentEventID);
    }

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
}