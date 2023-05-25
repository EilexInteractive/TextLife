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
	private int _SavedGamesCount = 0;					// Reference to the amount of saved games we have
	public int SavedGamesCount { get => _SavedGamesCount; }

	public ELoadType LoadType = ELoadType.LOADING;			// Current load type (dec)

	public override void _Ready()
	{
		base._Ready();
		_SavedGamesCount = GetSavedGameCount();				// Count the amount of save games
	}

	/// <summary>
	/// Creates a save of the current game that's active
	/// </summary>
	public void SaveGame()
	{
		// Get reference to the game controller & validate it
		GameController game = GetNode<GameController>("/root/GameController");
		if(game != null)
		{
			SaveGame save = new SaveGame();				// Create a new save file.
			save.Character = game.CurrentCharacter.CreateCharacterSave();			// Create the character save
			// Set the name of the saved game
			save.SaveName = (game.PlayerPrefs.LastSaveName != "" ? save.Character.LastName : game.CurrentCharacter.LastName);
			save.DateIndex = game.CurrentEventID;				// Get the current date index/event ID
			save.CurrentMonth = game.CurrentMonth;				// Save which month we are currently in

			string output = JsonConvert.SerializeObject(save);			// Convert the class to JSON

			// Get reference to the application folder
			string folderPath = game.ApplicationFolder;

			JsonSerializer serializer = new JsonSerializer();				// create a new serializer
			serializer.NullValueHandling = NullValueHandling.Ignore;			// Ignore any blank values


			// Write the JSON data to the file
			using(StreamWriter sw = new StreamWriter(folderPath + $"/Saves/{save.SaveName}.ei"))
			{
				using(JsonWriter jw = new JsonTextWriter(sw))
				{
					serializer.Serialize(jw, save);
				}
			}

			game.PlayerPrefs.LastSaveName = save.SaveName;				// Set the last save name
			SavePlayerPrefs();				// Save the player prefs
		}
	}


	/// <summary>
	/// Load the game the specified path
	/// </summary>
	/// <param name="filePath">Path to save game</param>
	/// <returns>If the game has loaded or not</returns>
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
					game.CurrentEventID = save.DateIndex;
					return true;
				}
			}
		}


		// TODO: Error handling

		return false;
	}

	/// <summary>
	/// Saves the player prefs to file
	/// </summary>
	public void SavePlayerPrefs()
	{
		// Get reference to the application folder path
		string applicationFolder = GetNode<GameController>("/root/GameController")?.ApplicationFolder;
		JsonSerializer serializer = new JsonSerializer();					// Create a new serializer
		serializer.NullValueHandling = NullValueHandling.Ignore;			// Ignore any blank files

		// Write the prefs JSON to file
		using(StreamWriter sw = new StreamWriter(applicationFolder + "/PlayerPrefs.ei"))
		{
			
			using(JsonWriter jsonWriter = new JsonTextWriter(sw))
			{
				serializer.Serialize(jsonWriter, GetNode<GameController>("/root/GameController")?.PlayerPrefs);
			}
		}
	}


	/// <summary>
	/// Counts how many saves we have
	/// </summary>
	/// <returns>The amount of saves in the save folder</returns>
	private int GetSavedGameCount()
	{
		// Get reference to the game controller & validate it
		GameController gameController = GetNode<GameController>("/root/GameController");
		if(gameController != null)
		{
			string savePath = gameController.ApplicationFolder + "/Saves";				// Get the location of the saves folder
			// Check that the save folder exist and count the amount of saves
			if(Directory.Exists(savePath))
			{
				string[] saves = Directory.GetFiles(savePath);
				return saves.Length;
			}
		}

		return 0;
	}
}