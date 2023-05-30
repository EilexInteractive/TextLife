using System;
using System.Collections.Generic;
using Godot;

public class RelationshipSave
{
    public string Character_1;
    public string Character_2;
    public float Meter;
    public int Type;

    public RelationshipSave()
    {}

    public RelationshipSave(string char1, string char2, float meter, ERelationshipType type)
    {
        Character_1 = char1;
        Character_2 = char2;
        Meter = meter;
        Type = (int)type;
    }

    public Relationship LoadRelationship(WorldController world)
    {
        if(world != null)
        {
            CharacterDetails character_1 = world.GetCharacterFromWorld(Character_1);
            CharacterDetails character_2 = world.GetCharacterFromWorld(Character_2);

            if(character_1 != null && character_2 != null)
            {
                return new Relationship(character_1, character_2, Meter, (ERelationshipType)Type);
            }
        }

        return null;
    }
}