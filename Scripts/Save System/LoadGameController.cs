using Godot;
using System;
using System.Collections.Generic;
using System.IO;



public partial class LoadGameController : TextureRect
{
	private List<LoadGameObject> _LoadGameObjects = new List<LoadGameObject>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		for(int i = 1; i <= 5; ++i)
			_LoadGameObjects.Add(GetNode<LoadGameObject>($"VBoxContainer/LoadGame_{i}"));
			
		GetLoadGames();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void GetLoadGames()
	{
		GameController gameController = GetNode<GameController>("/root/GameController");
		if(gameController != null)
		{
			string applicationFilePath = gameController.ApplicationFolder;
			string saveFolderPath = applicationFilePath + "/Saves";
			string[] files = Directory.GetFiles(saveFolderPath);
			if(files.Length <= 5)
			{
				for(int i = 0; i < files.Length; ++i)
				{
					string fileName = Path.GetFileName(files[i]);
					fileName = fileName.Replace(".ei", "");
					LoadGameObject loadObject = _LoadGameObjects[i];
					if(loadObject != null)
					{
						loadObject.SetSave(files[i], fileName);
					}
				}
			}
		}
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
