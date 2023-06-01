using System;
using System.Collections.Generic;
using Godot;

public partial class WorldEventMethods : Node
{

    public override void _Ready()
    {
        base._Ready();
        EventDatabase eventDb = GetNode<EventDatabase>("/root/EventDatabase");
        if(eventDb != null)
        {
            
        }
    }

    public void CountryWarStart(ELifeEventType type, WorldEventData data, ref LifeEventLog log)
    {
        Country origin = data.CountryOrigin;
        Country involved = data.InvolvedCountry;

        log.Text = log.Text.Replace("(origin)", origin.Name);
        log.Text = log.Text.Replace("(involved)", involved.Name);

        WorldController world = GetNode<WorldController>("/root/WorldController");
    }

    public void CountryWarUpdate(ELifeEventType type, WorldEventData data, ref LifeEventLog log)
    {
        Country origin = data.CountryOrigin;
        Country invovled = data.InvolvedCountry;

        log.Text = log.Text.Replace("(origin)", origin.Name);
        log.Text = log.Text.Replace("(involved)", invovled.Name);

        if(log.Text.Contains("(city)"))
        {
            WorldWarCity(ref log, origin, invovled);
        }
    }

    public void CountryWarEnd(ELifeEventType type, WorldEventData data, ref LifeEventLog log)
    {
        Country origin = data.CountryOrigin;
        Country involved = data.InvolvedCountry;

        RandomNumberGenerator rand = new RandomNumberGenerator();
        float randValue = rand.Randf();
        if(randValue > 0.5)
        {
            log.Text = log.Text.Replace("(win)", origin.Name);
            log.Text = log.Text.Replace("(loss)", involved.Name);
        } else 
        {
            log.Text = log.Text.Replace("(win)", involved.Name);
            log.Text = log.Text.Replace("(loss)", origin.Name);
        }
    }

    public void RelationshipEvent(LifeEventLog e)
    {

    }

    private void WorldWarCity(ref LifeEventLog log, Country invovled, Country origin)
    {
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();

        float randValue = rand.Randf();
        string city = "";
        if(randValue > 0.5f)
        {
            city = invovled.GetRandomState();
        } else 
        {
            city = origin.GetRandomState();
        }

        log.Text = log.Text.Replace("(city)", city);
    }
}