using System;
using System.Collections.Generic;
using Godot;

public enum EEventRequestStatus
{
    PENDING,
    ACCEPTED
}

public class LifeEventRequest
{
    private LifeEventLog _CorrespondingEvent;
    public LifeEventLog CorrespondingEvent { get => _CorrespondingEvent; }
    private CharacterDetails _ToCharacter;
    public CharacterDetails ToCharacter { get => _ToCharacter; }
    private CharacterDetails _FromCharacter;
    public CharacterDetails FromCharacter { get => _FromCharacter; }
    private EEventRequestStatus _EventStatus;
    public EEventRequestStatus EventStatus { get => _EventStatus; }

    public LifeEventRequest(LifeEventLog e, CharacterDetails to, CharacterDetails from)
    {
        _CorrespondingEvent = e;
        _ToCharacter = to;
        _FromCharacter = from;
        _EventStatus = EEventRequestStatus.PENDING;
    }

    public LifeEventRequest(LifeEventLog e, CharacterDetails to, CharacterDetails From, EEventRequestStatus status)
    {
        _CorrespondingEvent = e;
        _ToCharacter = to;
        _FromCharacter = From;
        _EventStatus = status;
    }

    public void AcceptEvent() => _EventStatus = EEventRequestStatus.ACCEPTED;
}