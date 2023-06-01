using Godot;
using System;
using System.Collections.Generic;

public partial class RelationshipRequestController : TextureRect
{
	[Export] private PackedScene _RequestPrefab;
	private List<Button> _SpawnedButtons = new List<Button>();
	private VBoxContainer _RelationFromList;
	public LifeEventRequest _ViewingRequest = null;

	private bool _IsVisible = false;

	public override void _Ready()
	{
		_RelationFromList = GetNode<VBoxContainer>("EventMessages/VBoxContainer");

		this.VisibilityChanged += ToggleRequestView;
	}

	public void ToggleRequestView()
	{
		_IsVisible = !_IsVisible;
		if(_IsVisible)
		{
			GetOutstandingRequest();
		} else 
		{
			ClearAllRequest();
		}
	}

	private void GetOutstandingRequest()
	{
		GameController game = GetNode<GameController>("/root/GameController");
		Label eventText = GetNode<Label>("EventData/VBoxContainer/Label");
		
		if(game != null)
		{
			CharacterDetails currentCharacter = game.CurrentCharacter;
			if(currentCharacter != null)
			{
				foreach(var request in currentCharacter.PendingRelationshipEvents)
				{
					RelationshipEventFrom requestEvent = _RequestPrefab.Instantiate<RelationshipEventFrom>();
					if(requestEvent != null)
					{
						requestEvent.SetEvent(request, eventText, this);
						_RelationFromList.AddChild(requestEvent);
					}
				}
			}
		}
	}

	private void ClearAllRequest()
	{
		foreach(var item in _SpawnedButtons)
		{
			item.QueueFree();
		}

		_SpawnedButtons.Clear();
	}

	public void OnCloseRelationshipPressed()
	{
		this.Visible = false;
	}

	public void OnAcceptRequest()
	{
		GameController game = GetNode<GameController>("/root/GameController");
		if(game != null)
		{
			CharacterDetails currentCharacter = game.CurrentCharacter;
			if(currentCharacter != null)
			{
				currentCharacter.AcceptLifeEvent(_ViewingRequest, game.CurrentEventID);
				GetNode<EventContainer>("/root/CanvasGroup/ColorRect/TextureRect/EventLog").UpdateEvents();
			}
		}
	}


}
