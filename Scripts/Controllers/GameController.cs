using Godot;
using System;
using System.Collections.Generic;
public partial class GameController : Node
{
    public string ApplicationFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Eilex Interactive/TextLife";
    public PlayerPrefs PlayerPrefs;
	public CharacterDetails CurrentCharacter;
    
    private int _Month = 1;
    public int CurrentMonth { get => _Month; }
    
    public int CurrentEventID = 0;

    public string ReturnTo = "";

    private List<CharacterDetails> _InWorldCharacters = new List<CharacterDetails>();
    public List<CharacterDetails> InWorldCharacters { get => _InWorldCharacters; }

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
}
