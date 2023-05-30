using Godot;
using System;

public partial class RelationshipEventFrom : Button
{
	private LifeEventRequest _CorrespondingEvent;

	private Label _FromLabel;

	public override void _Ready()
	{
		_FromLabel = GetNode<Label>("Label");
	}

	public void SetEvent(LifeEventRequest e)
	{
		_CorrespondingEvent = e;
		if(_FromLabel != null)
		{
			_FromLabel.Text = e.FromCharacter.FirstName + " " + e.FromCharacter.LastName;
		}
	}
}
