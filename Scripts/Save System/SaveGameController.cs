using Godot;
using System;
using Newtonsoft.Json;
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
			save.Character = game.CurrentCharacter;
			save.DateIndex = game.CurrentEventID;

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