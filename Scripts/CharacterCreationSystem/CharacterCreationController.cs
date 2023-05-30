using Godot;
using System;

public partial class CharacterCreationController : TextureRect
{

	[Export] private PackedScene _MainGameView;
	[Export] private PackedScene _MainMenuView;

	private LineEdit _CharacterNameEdit;
	private Label _LocationEdit;
	private OptionButton _SexOption;
	private LocationSelectController _LocationContainer;

	private CharacterDetails _CreatedCharacter;

    public override void _Ready()
    {
        base._Ready();
		_CharacterNameEdit = GetNode<LineEdit>("CharacterName");
		_LocationEdit = GetNode<Label>("Location/Label");
		_SexOption = GetNode<OptionButton>("Sex");
		_LocationContainer = GetNode<LocationSelectController>("LocationContainer");

		Randomize();
    }

	/// <summary>
	/// Generate a new random character
	/// </summary>
	private void Randomize()
	{
		// Get reference to the character generator & validate it
		CharacterGenerator generator = GetNode<CharacterGenerator>("/root/CharacterGenerator");
		if(generator != null)
		{
			// Generate a new random baby and validate it
			_CreatedCharacter = generator.GenerateRandomCharacter(true);				
			if(_CreatedCharacter != null)
			{
				// Display Name combined
				if(_CharacterNameEdit != null)
					_CharacterNameEdit.Text = $"{_CreatedCharacter.FirstName} {_CreatedCharacter.LastName}";
				
				// Display Location combined
				if(_LocationEdit != null)
					_LocationEdit.Text = $"{_CreatedCharacter.State}, {_CreatedCharacter.Country.Name}";

				// Display Sex
				if(_SexOption != null)
				{
					if(_CreatedCharacter.Sex == ESex.MALE)
					{
						_SexOption.Selected = 0;
					} else 
					{
						_SexOption.Selected = 1;
					}
				}
			}
		}
	}

	public void OnRandomizePressed()
	{
		Randomize();
	}

	public void StartGamePressed()
	{
		WorldController world = GetNode<WorldController>("/root/WorldController");
		GameController game = GetNode<GameController>("/root/GameController");
		if(world != null && game != null)
		{
			string[] characterName = _CharacterNameEdit.Text.Split();
			_CreatedCharacter.SetCharacterName(characterName);

			string[] location = _LocationEdit.Text.Split();
			location[0].Replace(",", "");

			Country selectedLocation = null;

			CountryDatabase locationDatabase = GetNode<CountryDatabase>("/root/CountryDatabase");
			if(locationDatabase != null)
				selectedLocation = locationDatabase.GetCountryFromName(location[1]);

			if(selectedLocation != null)
				_CreatedCharacter.SetLocation(selectedLocation, location[0]);

			string selectedSex = _SexOption.GetItemText(_SexOption.GetSelectedId());

			if(selectedSex == "Male")
			{
				_CreatedCharacter.SetSex(ESex.MALE);
			} else 
			{
				_CreatedCharacter.SetSex(ESex.FEMALE);
			}

			world.AddCharacterToWorld(_CreatedCharacter);
			
			Relationship parentA = _CreatedCharacter.Relationships[0];
			Relationship parentB = _CreatedCharacter.Relationships[1];

			if(parentA.Character_1 == _CreatedCharacter)
			{
				world.AddCharacterToWorld(parentA.Character_2);
			} else 
			{
				world.AddCharacterToWorld(parentA.Character_1);
			}

			if(parentB.Character_1 == _CreatedCharacter)
			{
				world.AddCharacterToWorld(parentB.Character_2);
			} else 
			{
				world.AddCharacterToWorld(parentB.Character_1);
			}

			game.PlayerPrefs.LastSaveName = characterName[1];

			game.CurrentCharacter = _CreatedCharacter;
			GetNode<SaveGameController>("/root/SaveGameController")?.SaveGame();
		}

		GetTree().ChangeSceneToPacked(_MainGameView); 

	}

	public void OpenLocationContainer()
	{
		if(_LocationContainer != null)
			_LocationContainer.Visible = true;
	}

	public void OnBackPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
	}
}
