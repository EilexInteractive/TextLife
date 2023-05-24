using Godot;
using System;

public partial class Toggle : Label
{
	private ToggleGroup _Owner = null;
	private TextureButton _CheckedButton;

	[Export] private Texture2D _CheckedTexture;
	[Export] private Texture2D _UncheckedTexture;

	[Export] private bool _IsChecked = false;

    public override void _Ready()
    {
        base._Ready();
		

		_CheckedButton = GetNode<TextureButton>("TextureButton");
		ToggleBtn();
		_CheckedButton.Pressed += FlipButton;
    }

	public void FlipButton()
	{
		_IsChecked = !_IsChecked;
		ToggleBtn();

		if(_Owner != null)
			_Owner.SetSelectedToggle(this);
		
	}

	public void UncheckButton()
	{
		_IsChecked = false;
		ToggleBtn();
	}

	public void ToggleBtn()
	{
		if(_IsChecked)
		{
			_CheckedButton.TextureNormal = _CheckedTexture;
		}  else
		{
			_CheckedButton.TextureNormal = _UncheckedTexture;
		}
	}
	public void SetOwner(ToggleGroup group) => _Owner = group;
	public string GetToggleValue() => this.Text;
}
