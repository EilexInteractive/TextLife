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

    public float _RelationshipMeter = 0.5f;
    private const float MIN_RELATIONSHIP_REDUCTION = 0.03f;
    private const float MAX_RELATIONSHIP_REDUCTION = 0.1f;

    public Relationship(CharacterDetails a, CharacterDetails b, float meter = 0.5f, ERelationshipType type = ERelationshipType.ACQUAINTANCE)
    {
        _Character_1 = a;
        _Character_2 = b;
        _RelationshipType = type;
        _RelationshipMeter = meter;
    }

    public override string ToString()
    {
        return Character_1.FirstName + " " + Character_2.FirstName;
    }

    public void ReduceRelationship()
    {
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();

        float reductionAmount = rand.RandfRange(MIN_RELATIONSHIP_REDUCTION, MAX_RELATIONSHIP_REDUCTION);
        _RelationshipMeter -= reductionAmount;
    }
}

