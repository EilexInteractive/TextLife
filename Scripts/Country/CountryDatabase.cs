using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public partial class CountryDatabase : Node
{
    const string FILE_LOCATION = "res://Database/Countries.json";
    private List<Country> _Countries = new List<Country>();

    public override void _Ready()
    {
        base._Ready();
        LoadCountries();
    }

    public Country GetRandomCountry()
    {
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();
        return _Countries[rand.RandiRange(0, _Countries.Count - 1)];
    }

    public Country GetCountryFromName(string name)
    {
        foreach(var country in _Countries)
            if(country.Name == name)
                return country;

        return null; 
    }

    public string GetState(Country country)
    {
        RandomNumberGenerator random = new RandomNumberGenerator();
        random.Randomize();
        return country.States[random.RandiRange(0, country.States.Count - 1)];
    }

    private void LoadCountries()
    {
        var file = FileAccess.Open(FILE_LOCATION, FileAccess.ModeFlags.Read);
        Country country = null;
        StateContainer state = null;
        if (file != null && file.IsOpen())
        {
            string text = file.GetAsText();
            var objects = JArray.Parse(text);
            foreach (var obj in objects)
            {
                JToken token = obj;
                state = JsonConvert.DeserializeObject<StateContainer>(token.ToString());
                if (!ContainsCountry(state.Country))
                {
                    country = new Country();
                    country.Name = state.Country;
                    _Countries.Add(country);
                } else
                {
                    country = GetCountryFromString(state.Country);
                }

                if (state != null && country != null)
                {
                    country.States.Add(state.State);
                }
            }


            

        }

        file.Close();
    }

    private Country GetCountryFromString(string countryName)
    {
        foreach(var country in _Countries)
        {
            if (countryName == country.Name)
                return country;
        }

        return null;
    }

    private bool ContainsCountry(string countryName)
    {
        foreach(var country in _Countries)
        {
            if (country.Name == countryName)
                return true;
        }

        return false;
    }
}

public class StateContainer
{
    public string Country;
    public string State;
}