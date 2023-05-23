using Godot;
using System;
using System.IO;

public partial class MainMenuController : TextureRect
{
	[Export] private PackedScene _NewGameScene;

	private Label _QuickSaveName;
	private TextureButton _ContinueBtn;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_QuickSaveName = GetNode<Label>("QuickLoad");
		_ContinueBtn = GetNode<TextureButton>("VBoxContainer/ContinueBtn");
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
			_QuickSaveName.Text = lastSaveName;
			_ContinueBtn.Disabled = false;
		}
	}
	
}
