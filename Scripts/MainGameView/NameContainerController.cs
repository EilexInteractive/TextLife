using Godot;
using System;

public partial class NameContainerController : TextureRect
{
	[Export] private Texture2D _MaleSex;
	[Export] private Texture2D _FemaleSex;

	private TextureRect _GenderIcon;
	private Label _NameText;
	private Label _LocationText;
	private Label _AgeText;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_GenderIcon = GetNode<TextureRect>("GenderIcon");
		_NameText = GetNode<Label>("PlayerName");
		_LocationText = GetNode<Label>("Location");
		_AgeText = GetNode<Label>("Age");


		UpdateDetails();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void UpdateDetails()
    {
		if (_GenderIcon == null || _NameText == null || _LocationText == null || _AgeText == null)
			return;

		GameController controller = GetNode<GameController>("/root/GameController");
		if (controller != null)
		{
			CharacterDetails character = controller.CurrentCharacter;
			if (character != null)
			{

				if (character.Sex == ESex.MALE)
				{
					_GenderIcon.Texture = _MaleSex;
				}
				else if (character.Sex == ESex.FEMALE)
				{
					_GenderIcon.Texture = _FemaleSex;
				}
				
				_NameText.Text = $"{character.FirstName} {character.LastName}";
				_LocationText.Text = $"{character.State}, {character.Country.Name}";
				_AgeText.Text = character.GetAgeAsString();
			}
		}
	}
}
