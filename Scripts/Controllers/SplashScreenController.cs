using Godot;
using System;
using System.IO;
using Newtonsoft.Json;

public partial class SplashScreenController : Timer
{
	[Export] private PackedScene _SceneToLoad;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ValidateFolders();
		LoadPlayerPrefs();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void ValidateFolders()
	{
		string saveFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Eilex Interactive/TextLife/Saves";
		if(!Directory.Exists(saveFolder))
		{
			Directory.CreateDirectory(saveFolder);
		}
	}

	private void LoadPlayerPrefs()
	{
		string applicationFolder = GetNode<GameController>("/root/GameController")?.ApplicationFolder;
		if(Directory.Exists(applicationFolder))
		{
			if(File.Exists(applicationFolder + "/PlayerPrefs.ei"))
			{
				using(StreamReader sr = new StreamReader(applicationFolder + "/PlayerPrefs.ei"))
				{
					string text = sr.ReadToEnd();
					PlayerPrefs prefs = JsonConvert.DeserializeObject<PlayerPrefs>(text);
					GetNode<GameController>("/root/GameController").PlayerPrefs = prefs;
				}
			} else 
			{
				GetNode<GameController>("/root/GameController").PlayerPrefs = new PlayerPrefs();
				GetNode<SaveGameController>("/root/SaveGameController")?.SavePlayerPrefs();
			}
		}
	}

	public void OnTimeOut()
    {
		if(_SceneToLoad != null)
        {
			GetTree().ChangeSceneToPacked(_SceneToLoad);
        }
    }

}
