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
				GD.Print($"{game.CurrentCharacter.FirstName} {game.CurrentCharacter.LastName}");
            }
        }
	}

	public void OnAdvanceGame()
	{
		// Get the game controller and validate it
		GameController game = GetNode<GameController>("/root/GameController");
		if(game == null)
			return;

		game.IncrementMonth();
		CharacterDetails currentCharacter = game.CurrentCharacter;
		if(currentCharacter != null)
			currentCharacter.AgeUp(game);

		EventContainer eventContainer = GetNode<EventContainer>("TextureRect/EventLog");
		if(eventContainer != null)
			eventContainer.UpdateEvents();

		NameContainerController nameContainer = GetNode<NameContainerController>("TextureRect/NameContainer");
		if(nameContainer != null)
			nameContainer.UpdateDetails();
	}

}
