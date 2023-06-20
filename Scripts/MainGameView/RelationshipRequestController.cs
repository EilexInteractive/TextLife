using Godot;
using System;
using System.Collections.Generic;

public partial class RelationshipRequestController : TextureRect
{
	[Export] private PackedScene _RequestPrefab;				// Reference to the prefab
	private List<Button> _SpawnedButtons = new List<Button>();			// Reference to all the spawned buttons
	private VBoxContainer _RelationFromList;				// Reference to the relationship list view
	public LifeEventRequest _ViewingRequest = null;			// Reference to the request we are currently viewing

	private bool _IsVisible = false;					// If this panel is visible

	public override void _Ready()
	{
		_RelationFromList = GetNode<VBoxContainer>("EventMessages/VBoxContainer");

		this.VisibilityChanged += ToggleRequestView;
	}

	/// <summary>
	/// Toggles the request view
	/// </summary>
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

	/// <summary>
	/// Gets reference to all the request that are currently pending
	/// </summary>
	private void GetOutstandingRequest()
	{
		GameController game = GetNode<GameController>("/root/GameController");			// Get reference to the game controller
		Label eventText = GetNode<Label>("EventData/VBoxContainer/Label");			// Get reference to where the request will display
		

		if(game != null)
		{
			CharacterDetails currentCharacter = game.CurrentCharacter;			// Get reference to the player character
			if(currentCharacter != null)
			{
				// Loop through each pending request
				foreach(var request in currentCharacter.PendingRelationshipEvents)
				{
					// Create the request event data
					RelationshipEventFrom requestEvent = _RequestPrefab.Instantiate<RelationshipEventFrom>();
					if(requestEvent != null)
					{
						requestEvent.SetEvent(request, eventText, this);			// Set the event
						_RelationFromList.AddChild(requestEvent);					// Add the event to the world
						_SpawnedButtons.Add(requestEvent);							// Add to the spawned list
					}
				}
			}
		}
	}

	/// <summary>
	/// Deletes all the request that have spawned
	/// </summary>
	private void ClearAllRequest()
	{
		foreach(var item in _SpawnedButtons)
		{
			item.QueueFree();
		}

		_SpawnedButtons.Clear();
	}

	/// <summary>
	/// Event called when the panel closes
	/// </summary>
	public void OnCloseRelationshipPressed()
	{
		this.Visible = false;
	}

	/// <summary>
	/// Event for when we press the accept event button
	/// </summary>
	public void OnAcceptRequest()
	{
		GameController game = GetNode<GameController>("/root/GameController");				// Get the reference to the game controller
		if(game != null && _ViewingRequest != null)
		{
			_ViewingRequest.AcceptEvent(game.CurrentEventID);				// Accept the event
			// Update the event data container
			GetNode<EventContainer>("/root/CanvasGroup/ColorRect/TextureRect/EventLog").UpdateEvents();

			// Update the pending request view
			ClearAllRequest();				
			GetOutstandingRequest();
		}
	}

	/// <summary>
	/// Event for when the player declines a request
	/// </summary>
	public void OnDeclinePressed()
	{
		GameController game = GetNode<GameController>("/root/GameController");				// Get reference to the game controller
		if(game != null)
		{
			CharacterDetails currentCharacter = game.CurrentCharacter;			// Get reference to the player character
			if(currentCharacter != null)
			{
				currentCharacter.DeclineLifeEvent(_ViewingRequest);			// Decline the event


				// Update the pending request view
				ClearAllRequest();
				GetOutstandingRequest();
			}
		}
	}

	public void AdvanceDay()
	{
		this.Visible = false;
		_ViewingRequest = null;
	}


}
