using Godot;
using System;

public partial class CharacterStatsContainerController : Node
{
	private Label _SmartsText;
	private Label _PhysicalHealthText;
	private Label _MentalHealthText;
	private Label _SocialText;
	private Label _LooksText;
	private Label _TiredText;

	private Label _MoneyText;

    public override void _Ready()
    {
        base._Ready();
		_SmartsText = GetNode<Label>("SmartsIndicator");
		_PhysicalHealthText = GetNode<Label>("PhysicalHealthIndicator");
		_MentalHealthText = GetNode<Label>("MentalHealthIndicator");
		_SocialText = GetNode<Label>("SocialIndicator");
		_LooksText = GetNode<Label>("LooksIndicator");
		_MoneyText = GetNode<Label>("Money");
		_TiredText = GetNode<Label>("TiredIndicator");

		UpdateStats();
    }

	public void UpdateStats()
	{
		GameController game = GetNode<GameController>("/root/GameController");
		if(game != null)
		{
			CharacterStats stats = game.CurrentCharacter?.Stats;
			if(stats != null)
			{
				if(_SmartsText != null)
					_SmartsText.Text = $"{Mathf.FloorToInt(stats.Smarts).ToString()}%";
				
				if(_PhysicalHealthText != null)
					_PhysicalHealthText.Text = $"{Mathf.FloorToInt(stats.PhysicalHealth).ToString()}%";

				if(_MentalHealthText != null)
					_MentalHealthText.Text = $"{Mathf.FloorToInt(stats.MentalHealth).ToString()}%";

				if(_SocialText != null)
					_SocialText.Text = $"{Mathf.FloorToInt(stats.Social).ToString()}%";
				
				if(_LooksText != null)
					_LooksText.Text = $"{Mathf.FloorToInt(stats.Social).ToString()}%";

				if(_TiredText != null)
					_TiredText.Text = $"{Mathf.FloorToInt(stats.Tired).ToString()}%";
			}

			string money = game.CurrentCharacter?.GetMoneyAsString();
			_MoneyText.Text = money;

		}
	}
}
