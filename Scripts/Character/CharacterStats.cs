using System;
using Godot;

public class CharacterStats
{
    private float _Smarts;
    public float Smarts { get => _Smarts;}
    private float _PhysicalHealth;
    public float PhysicalHealth { get => _PhysicalHealth; }
    private float _MentalHealth;
    public float MentalHealth { get => _MentalHealth; }
    private float _Social;
    public float Social { get => _Social; }
    private float _Looks;
    public float Looks { get => _Looks; }

    public CharacterStats(float smarts, float physical, float mental, float social, float looks)
    {
        _Smarts = smarts;
        _PhysicalHealth = physical;
        _MentalHealth = mental;
        _Social = social;
        _Looks = looks;
    }
}