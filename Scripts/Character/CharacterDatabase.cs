using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public partial class CharacterDatabase : Node
{

    const string FILE_LOCATION = "res://Database/Names.json";
    

    private List<string> _MaleNames = new List<string>();
    private List<string> _FemaleNames = new List<string>();
    private List<string> _LastNames = new List<string>();

    public override void _Ready()
    {
        base._Ready();
        LoadNames();
    }

    public string GetRandomFirstName(ESex sex)
    {
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();
        if(sex == ESex.MALE)
        {
            return _MaleNames[rand.RandiRange(0, _MaleNames.Count - 1)];
        } else
        {
            return _FemaleNames[rand.RandiRange(0, _FemaleNames.Count - 1)];
        }
    }

    public string GetRandomLastName()
    {
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();
        return _LastNames[rand.RandiRange(0, _LastNames.Count - 1)];
    }

    private void LoadNames()
    {
        var file = Godot.FileAccess.Open(FILE_LOCATION, Godot.FileAccess.ModeFlags.Read);
        if(file != null)
        {
            if(file.IsOpen())
            {
                string text = file.GetAsText();
                var objects = JArray.Parse(text);
                foreach(var obj in objects)
                {
                    JToken token = obj;
                    CharacterName name = JsonConvert.DeserializeObject<CharacterName>(token.ToString());
                    if(name.Category == "Male")
                    {
                        _MaleNames.Add(name.Name);
                    } else if(name.Category == "Female")
                    {
                        _FemaleNames.Add(name.Name);
                    } else if(name.Category == "LastName")
                    {
                        _LastNames.Add(name.Name);
                    }
                }


                file.Close();
            }
        } else 
        {
            GD.Print("Failed to load JSON file: Names");
        }
    }
}

public class CharacterName
{
    public string Name;
    public string Category;
}