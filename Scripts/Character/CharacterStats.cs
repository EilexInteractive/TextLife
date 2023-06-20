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
    private float _Tired;
    public float Tired { get => _Tired; }
    private float _TiredModifier = 1.0f;

    public CharacterStats(float smarts, float physical, float mental, float social, float looks, float tired)
    {
        _Smarts = smarts;
        _PhysicalHealth = physical;
        _MentalHealth = mental;
        _Social = social;
        _Looks = looks;
        _Tired = tired;
    }

    public void ReduceTiredness(int amount)
    {
        _Tired -= (amount * _TiredModifier);
        if(_Tired < 0)
            _Tired = 0;
    }
}