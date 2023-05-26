using Godot;
using System;
using System.IO;

public partial class MainMenuController : TextureRect
{
	[Export] private PackedScene _NewGameScene;
	[Export] private PackedScene _GameView;
	[Export] private PackedScene _LoadGameView;
	[Export] private PackedScene _SettingsView;

	private Label _QuickSaveName;
	private TextureButton _ContinueBtn;
	private TextureButton _NewGameBtn;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_QuickSaveName = GetNode<Label>("QuickLoad");
		_ContinueBtn = GetNode<TextureButton>("VBoxContainer/ContinueBtn");
		_NewGameBtn = GetNode<TextureButton>("VBoxContainer/NewGameBtn");

		SaveGameController saveController = GetNode<SaveGameController>("/root/SaveGameController");
		if(saveController != null)
		{
			if(saveController.SavedGamesCount >= 5)
			{
				if(_NewGameBtn != null)
					_NewGameBtn.Disabled = true;
			}
		}

		GetLastSave();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnNewGamePressed()
    {
		GetTree().ChangeSceneToPacked(_NewGameScene);
    }

	private void GetLastSave()
	{
		string lastSaveName = GetNode<GameController>("/root/GameController")?.PlayerPrefs.LastSaveName;
		if(lastSaveName != "" && _QuickSaveName != null && _ContinueBtn != null)
		{
			SaveGameController saveController = GetNode<SaveGameController>("/root/SaveGameController");
			if(saveController != null)
			{
				if(saveController.HasSaveGame(lastSaveName))
				{
					_QuickSaveName.Text = lastSaveName;
					_ContinueBtn.Disabled = false;
				} else 
				{
					GetNode<GameController>("/root/GameController").PlayerPrefs.LastSaveName = "";
					saveController.SavePlayerPrefs();
				}
			}
		}
	}

	public void OnContinuePressed()
	{
		GameController game = GetNode<GameController>("/root/GameController");
		SaveGameController saveController = GetNode<SaveGameController>("/root/SaveGameController");

		if(game == null || saveController == null)
			return;

		string saveFilePath = GetNode<GameController>("/root/GameController")?.ApplicationFolder;
		saveFilePath += "/Saves/" + GetNode<GameController>("/root/GameController").PlayerPrefs.LastSaveName + ".ei";
		if(saveController.LoadGame(saveFilePath))
		{
			GetTree().ChangeSceneToPacked(_GameView);
		}
	}

	public void OnLoadGamePressed()
	{
		GameController gameController = GetNode<GameController>("/root/GameController");
		if(gameController != null)
		{
			gameController.ReturnTo = "MainMenu";
			GetTree().ChangeSceneToPacked(_LoadGameView);
		}
		
	}

	public void OnSettingsPressed()
	{
		GameController gameController = GetNode<GameController>("/root/GameController");
		if(gameController != null)
		{
			gameController.ReturnTo = "MainMenu";
			GetTree().ChangeSceneToPacked(_SettingsView);
		}
	}

	public void OnExitPressed()
	{
		GetTree().Quit();
	}
	
}
