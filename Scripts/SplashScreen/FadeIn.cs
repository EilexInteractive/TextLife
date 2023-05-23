using Godot;
using System;

public partial class FadeIn : TextureRect
{
	[Export] private float _FadeInTime;
	static private float _Time;
 	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_Time += 0.5f * (float)delta;
		Modulate = new Color(0, 0, 0, Mathf.Lerp(0, 255, _Time));
	}
}
