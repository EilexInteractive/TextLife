using Godot;
using System;

public partial class LoadGameController : TextureRect
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnBackPressed()
	{
		GameController gameController = GetNode<GameController>("/root/GameController");
		if(gameController != null)
		{
			if(gameController.ReturnTo == "MainMenu")
			{
				GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
			} else if(gameController.ReturnTo == "GameView")
			{
				GetTree().ChangeSceneToFile("res://Scenes/MainGameView.tscn");
			}
		}
	}
}
