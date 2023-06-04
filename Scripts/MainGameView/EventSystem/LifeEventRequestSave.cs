using Godot;
using System;
using System.Collections.Generic;

public class LifeEventRequestSave
{
    public LifeEventSave CorrespondingRequest;
    public string ToCharacter;
    public string FromCharacter;
    public int EventStatus;

    public LifeEventRequestSave(LifeEventSave e, CharacterDetails to, CharacterDetails from, EEventRequestStatus status)
    {
        CorrespondingRequest = e;
        ToCharacter = to.CharacterID;
        FromCharacter = from.CharacterID;
        EventStatus = (int)status;
    }

    public LifeEventRequestSave(LifeEventRequest request)
    {
        CorrespondingRequest = request.CorrespondingEvent.CreateEventSave();
        ToCharacter = request.ToCharacter.CharacterID;
        FromCharacter = request.FromCharacter.CharacterID;
        EventStatus = (int)request.EventStatus;
    }

    public LifeEventRequest LoadRequest()
    {
        WorldController world = new WorldController();
        if(world == null)
            return null;


        LifeEventLog log = CorrespondingRequest.LoadLifeEvent();
        CharacterDetails to = world.GetCharacterFromWorld(ToCharacter);
        CharacterDetails from = world.GetCharacterFromWorld(FromCharacter);
        EEventRequestStatus status = (EEventRequestStatus)EventStatus;
        return  new LifeEventRequest(log, to, from, status);
    }
}