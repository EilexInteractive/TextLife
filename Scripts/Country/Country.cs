using System;
using System.Collections.Generic;
using Godot;

public class Country
{
    public string Name;
    public List<string> States = new List<string>();

    public string GetRandomState()
    {
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();
        return States[rand.RandiRange(0, States.Count - 1)];
    }
}
