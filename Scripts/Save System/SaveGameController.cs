using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

public partial class SaveGameController : Node
{
	public void SaveGame()
	{
		GameController game = GetNode<GameController>("/root/GameController");
		if(game != null)
		{
			SaveGame save = new SaveGame();
			save.SaveName = game.CurrentCharacter.FirstName + " " + game.CurrentCharacter.LastName;
			save.Character = game.CurrentCharacter.CreateCharacterSave();
			save.DateIndex = game.CurrentEventID;

			string output = JsonConvert.SerializeObject(save);

			string folderPath = GetNode<GameController>("/root/GameController")?.ApplicationFolder;

			JsonSerializer serializer = new JsonSerializer();
			serializer.NullValueHandling = NullValueHandling.Ignore;

			using(StreamWriter sw = new StreamWriter(folderPath + $"/Saves/{save.SaveName}.ei"))
			{
				using(JsonWriter jw = new JsonTextWriter(sw))
				{
					jw.WriteStartArray();
					jw.WriteRawValue(JsonConvert.SerializeObject(save));
					jw.WriteEndArray();
					//serializer.Serialize(jw, save);
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
					var saveObject = JArray.Parse(text);
					JToken token = saveObject;
					SaveGame load = JsonConvert.DeserializeObject<SaveGame>(token.ToString());
					GD.Print(load.SaveName);
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
}