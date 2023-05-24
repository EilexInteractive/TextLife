using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

public enum ELoadType
{
	SAVING,
	LOADING
}

public partial class SaveGameController : Node
{
	private int _SavedGamesCount = 0;
	public int SavedGamesCount { get => _SavedGamesCount; }

	public ELoadType LoadType = ELoadType.LOADING;

	public override void _Ready()
	{
		base._Ready();
		_SavedGamesCount = GetSavedGameCount();
	}

	public void SaveGame()
	{
		GameController game = GetNode<GameController>("/root/GameController");
		if(game != null)
		{
			SaveGame save = new SaveGame();
			save.Character = game.CurrentCharacter.CreateCharacterSave();
			save.SaveName = (game.PlayerPrefs.LastSaveName != "" ? save.Character.LastName : game.CurrentCharacter.LastName);
			save.DateIndex = game.CurrentEventID;
			save.CurrentMonth = game.CurrentMonth;

			string output = JsonConvert.SerializeObject(save);

			string folderPath = GetNode<GameController>("/root/GameController")?.ApplicationFolder;

			JsonSerializer serializer = new JsonSerializer();
			serializer.NullValueHandling = NullValueHandling.Ignore;

			using(StreamWriter sw = new StreamWriter(folderPath + $"/Saves/{save.SaveName}.ei"))
			{
				using(JsonWriter jw = new JsonTextWriter(sw))
				{
					serializer.Serialize(jw, save);
				}
			}

			game.PlayerPrefs.LastSaveName = save.SaveName;
			SavePlayerPrefs();
		}
	}

	public bool LoadGame(string filePath)
	{
		GameController game = GetNode<GameController>("/root/GameController");
		if(game != null)
		{
			if(File.Exists(filePath))
			{
				using(StreamReader sr = new StreamReader(filePath))
				{
					
					string text = sr.ReadToEnd();
					SaveGame save = JsonConvert.DeserializeObject<SaveGame>(text);
					CharacterDetails character = save.Character.LoadCharacter(GetNode<CountryDatabase>("/root/CountryDatabase"));
					game.CurrentCharacter = character;
					return true;
				}
			}
		}

		return false;
	}

	public void SavePlayerPrefs()
	{
		string applicationFolder = GetNode<GameController>("/root/GameController").ApplicationFolder;
		JsonSerializer serializer = new JsonSerializer();
		serializer.NullValueHandling = NullValueHandling.Ignore;

		using(StreamWriter sw = new StreamWriter(applicationFolder + "/PlayerPrefs.ei"))
		{
			
			using(JsonWriter jsonWriter = new JsonTextWriter(sw))
			{
				serializer.Serialize(jsonWriter, GetNode<GameController>("/root/GameController")?.PlayerPrefs);
			}
		}
	}


	private int GetSavedGameCount()
	{
		GameController gameController = GetNode<GameController>("/root/GameController");
		if(gameController != null)
		{
			string savePath = gameController.ApplicationFolder + "/Saves";
			if(Directory.Exists(savePath))
			{
				string[] saves = Directory.GetFiles(savePath);
				return saves.Length;
			}
		}

		return 0;
	}
}