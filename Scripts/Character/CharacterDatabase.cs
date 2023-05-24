using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public partial class CharacterDatabase : Node
{

    const string FILE_LOCATION = "res://Database/Names.json";           // Location of the names database
    

    // List to all the names read in from the file
    private List<string> _MaleNames = new List<string>();
    private List<string> _FemaleNames = new List<string>();
    private List<string> _LastNames = new List<string>();

    public override void _Ready()
    {
        base._Ready();
        LoadNames();
    }

    /// <summary>
    /// Returns a random last name
    /// </summary>
    /// <param name="sex">Sex of the character</param>
    /// <returns>First Name</returns>
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

    /// <summary>
    /// Generates a last name for the character
    /// </summary>
    /// <returns>Last Name</returns>
    public string GetRandomLastName()
    {
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();
        return _LastNames[rand.RandiRange(0, _LastNames.Count - 1)];
    }

    /// <summary>
    /// Loads all the names from the database file
    /// </summary>
    private void LoadNames()
    {
        // Open the file and validate it
        var file = Godot.FileAccess.Open(FILE_LOCATION, Godot.FileAccess.ModeFlags.Read);       
        if(file != null)
        {
            // Check that the file is open
            if(file.IsOpen())
            {
                string text = file.GetAsText();                     // Get all the text as a string
                var objects = JArray.Parse(text);                   // Parse it as a json object
                // Loop through each of the objects
                foreach(var obj in objects)
                {
                    JToken token = obj;                 // Get reference to the token
                    // Get the name as the character
                    CharacterName name = JsonConvert.DeserializeObject<CharacterName>(token.ToString());

                    // Determine which list the name goes into
                    if(name.Category == "Male")
                    {
                        _MaleNames.Add(name.Name);
                    } else if(name.Category == "Female")
                    {
                        _FemaleNames.Add(name.Name);
                    } else if(name.Category == "LastName")
                    {
                        _LastNames.Add(name.Name);
                    } else 
                    {
                        _MaleNames.Add(name.Name);
                        _FemaleNames.Add(name.Name);
                        _LastNames.Add(name.Name);
                    }
                }


                file.Close();           // Close the file
            }
        } else 
        {
            // Error handling
            GD.Print("Failed to load JSON file: Names");
            // TODO: Display an error message
        }
    }
}

public class CharacterName
{
    public string Name;
    public string Category;
}