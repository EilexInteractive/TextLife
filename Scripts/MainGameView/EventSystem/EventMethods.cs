using System;
using System.Collections.Generic;
using Godot;

public partial class WorldEventMethods : Node
{

    public override void _Ready()
    {
        base._Ready();
    }

    public void CountryWarStart(ELifeEventType type, WorldEventData data, LifeEventLog log)
    {
        Country origin = data.CountryOrigin;
        Country involved = data.InvolvedCountry;

        log.Text = log.Text.Replace("(origin)", origin.Name);
        log.Text = log.Text.Replace("(against)", involved.Name);

        GameController game = GetNode<GameController>("/root/GameController");
        if(game != null)
        {
            game.AddEventToAllLogs(log);
        }
    }
}