using Godot;
using System;
using System.IO;

public partial class LoadGameObject : TextureButton
{
	private string FilePath;
	private string FileName;

	private Label _TextLabel;
	private TextureButton _DeleteBtn;

    public override void _Ready()
    {
        base._Ready();
		_TextLabel = GetNode<Label>("Label");
		_DeleteBtn = GetNode<TextureButton>("DeleteBtn");
		_DeleteBtn.Visible = false;

		this.Pressed += OnPressed;
    }

	public void SetSave(string filePath, string fileName)
	{
		FilePath = filePath;
		FileName = fileName;

		_TextLabel.Text = FileName;
		this.Disabled = false;
		_DeleteBtn.Visible = true;
		_DeleteBtn.Pressed += OnDeleteBtnPressed;
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

	public void OnDeleteBtnPressed()
	{
		if(File.Exists(FilePath))
		{
			File.Delete(FilePath);
			GetTree().ChangeSceneToFile("res://Scenes/LoadGame.tscn");
		}
	}
}
