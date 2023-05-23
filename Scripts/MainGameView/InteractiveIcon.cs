using Godot;
using System;

public partial class InteractiveIcon : TextureButton
{
	[Export] private PackedScene _View;				// Reference to the view we will go to

	public override void _Ready()
	{
		this.Pressed += OnButtonPressed;
	}

	public void OnButtonPressed()
	{
		if(_View != null)
			GetTree().ChangeSceneToPacked(_View);
	}

}
