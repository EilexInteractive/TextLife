using Godot;
using System;

public partial class SettingsMenuController : TextureRect
{
	private Toggle _AudioToggle;
	private Toggle _MusicToggle;

    public override void _Ready()
    {
        base._Ready();
		_AudioToggle = GetNode<Toggle>("VBoxContainer/Audio");
		_MusicToggle = GetNode<Toggle>("VBoxContainer/Music");

		LoadPrefs();
    }

	private void LoadPrefs()
	{
		GameController gameController = GetNode<GameController>("/root/GameController");
		if(gameController != null)
		{
			PlayerPrefs prefs = gameController.PlayerPrefs;
			if(prefs != null)
			{
				if(prefs.Audio == false)
					_AudioToggle.UncheckButton();

				if(prefs.Music == false)
					_MusicToggle.UncheckButton();
			}
		}
	}

	public void OnSavePressed()
	{
		GameController gameController = GetNode<GameController>("/root/GameController");
		if(gameController != null)
		{
			gameController.PlayerPrefs.Audio = _AudioToggle.IsChecked;
			gameController.PlayerPrefs.Music = _MusicToggle.IsChecked;

			SaveGameController saveController = GetNode<SaveGameController>("/root/SaveGameController");
			if(saveController != null)
				saveController.SavePlayerPrefs();

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
