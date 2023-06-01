using Godot;
using System;

public partial class RelationshipEventFrom : Button
{
	private LifeEventRequest _CorrespondingEvent;
	private RelationshipRequestController _RequestController;

	private Label _FromLabel;
	private Label _EventMessage;

	public override void _Ready()
	{
		this.Pressed += OnNameClicked;
	}

	public void SetEvent(LifeEventRequest e, Label eventText, RelationshipRequestController requestView)
	{
		_RequestController = requestView;
		_FromLabel = GetNode<Label>("Label");
		_CorrespondingEvent = e;
		if(_FromLabel != null)
		{
			_FromLabel.Text = e.FromCharacter.ToString();
		}
		_EventMessage = eventText;
	}

	public void OnNameClicked()
	{
		_EventMessage.Text = GetEventText();
		_RequestController._ViewingRequest = _CorrespondingEvent;
	}

	private string GetEventText()
	{
		
		_CorrespondingEvent.CorrespondingEvent.Text = _CorrespondingEvent.CorrespondingEvent.Text.Replace("(character)", _CorrespondingEvent.FromCharacter.ToString());
		return _CorrespondingEvent.CorrespondingEvent.Text;
	}
}
