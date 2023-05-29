using System;
using System.Collections.Generic;

public class SaveGame
{
	public string SaveName;
	public string PlayerID;							// Reference to the players character
	public int DateIndex; 
	public int CurrentMonth;
	public List<CharacterSave> CharactersInWorld = new List<CharacterSave>();
	public List<RelationshipSave> RelationshipsInWorld = new List<RelationshipSave>();
	public List<WorldEventSave> WorldEvents = new List<WorldEventSave>();
}
