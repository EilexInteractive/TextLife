using Godot;
using System;
using System.Collections.Generic;

public enum ERelationshipType
{
    ACQUAINTANCE = 0,
    FRIEND = 1,
    BEST_FRIEND = 2,
    PARENT = 3,
    SIBLING = 4,
    GRAND_PARENTS = 5,
    COUSIN = 6,
    ENEMY = 7,
    CHILD = 8,
    PARTNER = 9
}

public class Relationship
{
    private CharacterDetails _Character_1;
    public CharacterDetails Character_1 { get => _Character_1; }
    private CharacterDetails _Character_2;
    public CharacterDetails Character_2 { get => _Character_2; }
    private ERelationshipType _RelationshipType;
    public ERelationshipType RelationshipType { get => _RelationshipType; }

    public Relationship(CharacterDetails a, CharacterDetails b, ERelationshipType type = ERelationshipType.ACQUAINTANCE)
    {
        _Character_1 = a;
        _Character_2 = b;
        _RelationshipType = type;
    }

    public override string ToString()
    {
        return Character_1.FirstName + " " + Character_2.FirstName;
    }

}