using Godot;
using System;

public partial class MainSceneController : ColorRect
{
	// Called when the node enters the scene tree for the first time.
	public override void _EnterTree()
	{
		GameController game = GetNode<GameController>("/root/GameController");
		CharacterGenerator characterDb = GetNode<CharacterGenerator>("/root/CharacterGenerator");

		if(game != null && characterDb != null)
        {
			if(game.CurrentCharacter == null)
            {
				game.CurrentCharacter = characterDb.GenerateRandomCharacter(true);
            }
        }
	}

	public override void _Ready()
	{
		CheckForEvents();
	}

	public void OnAdvanceGame()
	{
		// Get the game controller and validate it
		GameController game = GetNode<GameController>("/root/GameController");
		WorldController world = GetNode<WorldController>("/root/WorldController");
		if(game == null || world == null)
			return;

		game.IncrementMonth();
		world.Advance();
			

		

		EventContainer eventContainer = GetNode<EventContainer>("TextureRect/EventLog");
		if(eventContainer != null)
			eventContainer.UpdateEvents();

		NameContainerController nameContainer = GetNode<NameContainerController>("TextureRect/NameContainer");
		if(nameContainer != null)
			nameContainer.UpdateDetails();

		SaveGameController saveController = GetNode<SaveGameController>("/root/SaveGameController");
		if(saveController != null)
			saveController.SaveGame();

		CheckForEvents();
	}

	public void OnRelationshipPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/RelationshipView.tscn");
	}

	public void OnExitToMenu()
	{
		SaveGameController saveController = GetNode<SaveGameController>("/root/SaveGameController");
		if(saveController != null)
		{
			saveController.SaveGame();
		}

		GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
	}

	public void OnLoadGamePressed()
	{
		GameController gameController = GetNode<GameController>("/root/GameController");
		if(gameController != null)
			gameController.ReturnTo = "GameView";

		SaveGameController saveController = GetNode<SaveGameController>("/root/SaveGameController");
		if(saveController != null)
			saveController.SaveGame();

		GetTree().ChangeSceneToFile("res://Scenes/LoadGame.tscn");
	}

	private void CheckForEvents()
	{
		GameController game = GetNode<GameController>("/root/GameController");
		if(game != null)
		{
			CharacterDetails currentCharacter = game.CurrentCharacter;
			if(currentCharacter != null)
			{
				if(currentCharacter.PendingRelationshipEvents.Count > 0)
				{
					GetNode<TextureRect>("TextureRect/RelationshipIcon/OutstandingRelationship").Visible = true;
				} else 
				{
					GetNode<TextureRect>("TextureRect/RelationshipIcon/OutstandingRelationship").Visible = false;
				}
			}
		}
	}

	public void OnRelationshipEventPressed()
	{
		GetNode<TextureRect>("TextureRect/RelationshipEventPanel").Visible = true;
	}
}
