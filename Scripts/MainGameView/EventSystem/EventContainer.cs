using Godot;
using System;
using System.Collections.Generic;

public partial class EventContainer : Node
{
	// Reference to the 2 elements we will be spawning
	[Export] private PackedScene _DateElement;
	[Export] private PackedScene _EventElement;


	// Child Nodes
	private VBoxContainer _EventContainer;

	// Reference to the objects spawned
	private List<Label> _SpawnedLabelEvents = new List<Label>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_EventContainer = GetNode<VBoxContainer>("ScrollContainer/VBoxContainer");
		UpdateEvents();
	}

	
	/// <summary>
	/// Updates the event log with text
	/// </summary>
	public void UpdateEvents()
	{
		//  Clear any spawned events
		if(_SpawnedLabelEvents.Count > 0)
		{
			foreach(var l in _SpawnedLabelEvents)
				l.QueueFree();

			_SpawnedLabelEvents.Clear();
		}

		// Get reference to the game controller & validate it
		GameController game = GetNode<GameController>("/root/GameController");
		if(game != null)
		{
			// Get reference to the current character & validate it.
			CharacterDetails currentCharacter = game.CurrentCharacter;
			if(currentCharacter != null)
			{
				// Create a new list for date events & reverse it
				List<LifeEventLog> dateEvents = new List<LifeEventLog>(currentCharacter.EventDates);
				dateEvents.Reverse();

				// Reference to the 
				List<LifeEventLog> eventLog = currentCharacter.LifeEventLog;

				foreach(var date in dateEvents)
				{
					List<LifeEventLog> usingEvents = new List<LifeEventLog>();
					Label createdDateLabel = _DateElement.Instantiate<Label>();
					if(createdDateLabel != null)
					{
						createdDateLabel.Text = date.Text;
					}

					_EventContainer.AddChild(createdDateLabel);
					_SpawnedLabelEvents.Add(createdDateLabel);

					foreach(var e in eventLog)
					{
						if(e.ID == date.ID)
							usingEvents.Add(e);
					}


					foreach(var e in usingEvents)
					{
						Label eventLabel = _EventElement.Instantiate<Label>();
						if(eventLabel != null)
						{
							eventLabel.Text = "- " + e.Text;
							_EventContainer.AddChild(eventLabel);
							_SpawnedLabelEvents.Add(eventLabel);
						}
					}
					
				}
			}
		}
	}

	/// <summary>
	/// Creates the label that will spawn
	/// </summary>
	/// <param name="eventLog">Reference to the event log so we can check the type</param>
	/// <returns>Created label</returns>
	private Label GetEventLabel(LifeEventLog eventLog)
	{
		// Validate the event log
		if(eventLog == null)
			return null;

		// Create the label based on the event type
		switch(eventLog.Type)
		{
			case ELifeEventType.DATE:
				return _DateElement.Instantiate<Label>();
			case ELifeEventType.EVENT:
				return _EventElement.Instantiate<Label>();
			default: 
				return _EventElement.Instantiate<Label>();
		}
	}
}
