using Godot;
using System;

public partial class LoadGameObject : TextureButton
{
	private string FilePath;
	private string FileName;

	private Label _TextLabel;

    public override void _Ready()
    {
        base._Ready();
		_TextLabel = GetNode<Label>("Label");

		this.Pressed += OnPressed;
    }

	public void SetSave(string filePath, string fileName)
	{
		FilePath = filePath;
		FileName = fileName;

		_TextLabel.Text = FileName;
		this.Disabled = false;
	}

	public void OnPressed()
	{
		SaveGameController saveController = GetNode<SaveGameController>("/root/SaveGameController");
		if(saveController != null)
		{
			if(saveController.LoadGame(FilePath))
				GetTree().ChangeSceneToFile("res://Scenes/MainGameView.tscn");
		}
	}
}
