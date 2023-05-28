using Godot;
using System;

public partial class RelationshipObject : Button
{
	private Label _CharacterName;
	private Label _Relation;
	private HSlider _RelationshipMeter;

	private Relationship _RelationshipData;

	public override void _Ready()
	{
		_CharacterName = GetNode<Label>("CharacterName");
		_Relation = GetNode<Label>("Relation");
		_RelationshipMeter = GetNode<HSlider>("Meter");
	}

	public void Setup(Relationship relation)
	{
		GameController game = GetNode<GameController>("/root/GameController");
		if(relation != null && game != null)
		{
			CharacterDetails playerCharacter = game.CurrentCharacter;
			_RelationshipData = relation;
			CharacterDetails relationCharacter = null;
			if(playerCharacter == _RelationshipData.Character_1)
			{
				relationCharacter = _RelationshipData.Character_2;
			} else 
			{
				relationCharacter = _RelationshipData.Character_1;
			}

			if(relationCharacter != null)
			{
				_CharacterName.Text = relationCharacter.FirstName + " " + relationCharacter.LastName;
				_Relation.Text = GetRelationshipAsString(_RelationshipData.RelationshipType, relationCharacter.Sex);

			}
		}
	}

	private string GetRelationshipAsString(ERelationshipType type, ESex sex)
	{
		switch(type)
		{
			case ERelationshipType.ACQUAINTANCE:
				return "Acquaintance";
			case ERelationshipType.FRIEND:
				return "Friend";
			case ERelationshipType.BEST_FRIEND:
				return "Best Friend";
			case ERelationshipType.PARENT:
				if(sex == ESex.MALE)
				{
					return "Father";
				} else 
				{
					return "Mother";
				}
			case ERelationshipType.SIBLING:
				if(sex == ESex.MALE)
				{
					return "Brother";
				} else
				{
					return "Sister";
				}
			case ERelationshipType.GRAND_PARENTS:
				if(sex == ESex.MALE)
				{
					return "Grandfather";
				} else 
				{
					return "GrandMother";
				}
			case ERelationshipType.COUSIN:
				return "Cousin";
			case ERelationshipType.ENEMY:
				return "Enemy";
			case ERelationshipType.CHILD:
				if(sex == ESex.MALE)
				{
					return "Son";
				} else 
				{
					return "Daughter";
				}
			case ERelationshipType.PARTNER:
				if(sex == ESex.MALE)
				{
					return "Boyfriend";
				} else 
				{
					return "Girlfriend";
				}
			default:
				return "Friend";
		}
	}

}
