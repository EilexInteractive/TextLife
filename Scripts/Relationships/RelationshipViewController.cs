using Godot;
using System;
using System.Collections.Generic;

public partial class RelationshipViewController : Node
{
	[Export] private PackedScene _RelationshipPrefab;
	private VBoxContainer _RelationshipContainer;

    public override void _Ready()
    {
        base._Ready();

		_RelationshipContainer = GetNode<VBoxContainer>("TextureRect/Background/ScrollContainer/RelationContainer");

		UpdateRelationships();
    }

	/// <summary>
	/// Gets all relationships and loads them into the scene
	/// </summary>
	private void UpdateRelationships()
	{
		GameController gameController = GetNode<GameController>("/root/GameController");			// Get reference to the game controller
		// Validate the game controller and the relationship controller
		if(gameController != null && _RelationshipContainer != null)
		{
			// Get reference to the current character & validate it
			CharacterDetails currentCharacter = gameController.CurrentCharacter;
			if(currentCharacter != null)
			{
				// Loop through each relationship and load it into the relationship
				foreach(var relationship in currentCharacter.Relationships)
				{
					RelationshipObject relationObj = _RelationshipPrefab.Instantiate<RelationshipObject>();
					_RelationshipContainer.AddChild(relationObj);
				}
			} else 
			{
				GD.Print("An Error Ocurred: No current character");
			}
		} else 
		{
			if(_RelationshipContainer == null)
			{
				GD.Print("An Error Occured: No relationship container");
			}
		}
	}

	public void OnReturnPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/MainGameView.tscn");
	}
}
