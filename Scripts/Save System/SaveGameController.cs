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
		WorldController world = GetNode<WorldController>("/root/WorldController");
		if(game != null && world != null)
		{
			SaveGame save = new SaveGame();				// Create a new save file.
			save.PlayerID = game.CurrentCharacter.CharacterID;
			// Set the name of the saved game
			save.SaveName = (game.PlayerPrefs.LastSaveName != "" ? game.PlayerPrefs.LastSaveName : game.CurrentCharacter.LastName);
			save.DateIndex = game.CurrentEventID;				// Get the current date index/event ID
			save.CurrentMonth = game.CurrentMonth;				// Save which month we are currently in

			foreach(var character in world.InWorldCharacters)
			{
				foreach(var relationship in character.Relationships)
				{
					RelationshipSave relationshipSave = new RelationshipSave(relationship.Character_1.CharacterID, relationship.Character_2.CharacterID, relationship._RelationshipMeter, relationship.RelationshipType);
					bool alreadyInSave = false;

					foreach(var saveRel in save.RelationshipsInWorld)
					{
						if(saveRel.Character_1 == relationship.Character_1.CharacterID && saveRel.Character_2 == relationship.Character_2.CharacterID)
						{
							alreadyInSave = true;
							break;
						} else if(saveRel.Character_2 == relationship.Character_1.CharacterID && saveRel.Character_1 == relationship.Character_2.CharacterID)
						{
							alreadyInSave = true;
							break;
						}
					}

					if(!alreadyInSave)
						save.RelationshipsInWorld.Add(relationshipSave);
				}

				save.CharactersInWorld.Add(character.CreateCharacterSave());
			}

			save.WorldEvents = world.GetWorldEventsAsSave();

			

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
		WorldController world = GetNode<WorldController>("/root/WorldController");
		if(game != null && world != null)
		{
			if(File.Exists(filePath))
			{
				using(StreamReader sr = new StreamReader(filePath))
				{
					
					string text = sr.ReadToEnd();
					SaveGame save = JsonConvert.DeserializeObject<SaveGame>(text);
					foreach(var character in save.CharactersInWorld)
					{
						CharacterDetails loadCharacter = character.LoadCharacter(GetNode<CountryDatabase>("/root/CountryDatabase"));
						if(loadCharacter != null)
							world.AddCharacterToWorld(loadCharacter);

						if(loadCharacter.CharacterID == save.PlayerID)
						{
							game.CurrentCharacter = loadCharacter;
						}
					}

					foreach(var relationship in save.RelationshipsInWorld)
					{
						Relationship loadedRelationship = relationship.LoadRelationship(world);
						if(loadedRelationship != null)
						{
							if(!loadedRelationship.Character_1.HasRelationship(loadedRelationship.Character_1.CharacterID, loadedRelationship.Character_2.CharacterID))
								loadedRelationship.Character_1.AddRelationship(loadedRelationship);
								
							if(!loadedRelationship.Character_2.HasRelationship(loadedRelationship.Character_1.CharacterID, loadedRelationship.Character_2.CharacterID))
								loadedRelationship.Character_2.AddRelationship(loadedRelationship);
						}
					}

					CountryDatabase countryDb = GetNode<CountryDatabase>("/root/CountryDatabase");

					foreach(var e in save.WorldEvents)
					{
						Country origin = countryDb.GetCountryFromName(e.OriginCountryName);
						Country involved = null;

						if(e.InvolvedCountryName != "")
						{
							involved = countryDb.GetCountryFromName(e.InvolvedCountryName);
						}

						WorldEventData data = new WorldEventData(origin, involved);

						foreach(var lifeEvent in e.Events)
						{
							LifeEventLog life = lifeEvent.LoadLifeEvent();
							data.AddEvent(life, false);
						}
						
					}

					

					// Set the main character
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

	public bool HasSaveGame(string fileName)
	{
		GameController gameController = GetNode<GameController>("/root/GameController");
		if(gameController != null)
		{
			string savePath = gameController.ApplicationFolder + "/Saves";
			if(Directory.Exists(savePath))
			{
				string[] saves = Directory.GetFiles(savePath);
				foreach(var save in saves)
				{
					string file = Path.GetFileName(save);
					file = file.Replace(".ei", "");
					if(file == fileName)
						return true;

				}
			}
		}

		return false;
	}
}