using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class CharacterGenerator : Node
{

    // === Age Properties === //
    private int MinAgeModifier = 3;
    private int MaxAgeModifier = 6;

    // === Baby Stats Properties === //
    private float MinBabySmarts = 5;
    private float  MaxBabySmarts = 30;
    private float MinBabyPhysicalHealth = 20;
    private float MaxBabyPhysicalHealth = 90;
    private float MinBabyMentalHealth = 70;
    private float MaxBabyMentalHealth = 100;
    private float MinBabySocial = 80;
    private float MaxBabySocial = 100;
    private float MinBabyLooks = 5;
    private float MaxBabyLooks = 10;

    // === Adult Properties === //
    private float MinAdultSmarts = 25;
    private float MaxAdultSmarts = 100;
    private float MinAdultPhysicalHealth = 3;
    private float MaxAdultPhysicalHealth = 100;
    private float MinAdultMentalHealth = 3;
    private float MaxAdultMentalHealth = 100;
    private float MinAdultSocial = 3;
    private float MaxAdultSocial = 100;
    private float MinAdultLooks = 10;
    private float MaxAdultLooks = 100;

    /// <summary>
    /// Generates a random character at age 0
    /// </summary>
    /// <param name="randomLastName">If we are generating a random name</param>
    /// <param name="existingLastName">If true; current last name to set</param>
    /// <returns>New character details</returns>
    public CharacterDetails GenerateRandomCharacter(bool isBaby, bool randomLastName = false, string existingLastName = "")
    {
        // Predefine character properties
        string firstName;
        string lastName;
        Country country;
        string state;
        ESex sex;

        // Create a random number generate to generate randomness
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();

        // Get a random value to determine the sex
        float genderRand = rand.Randf();
        if(genderRand > 0.5)
        {
            sex = ESex.MALE;
        } else
        {
            sex = ESex.FEMALE;
        }

        // Get a random first name
        firstName = GetNode<CharacterDatabase>("/root/CharacterDatabase").GetRandomFirstName(sex);
        // If we are using a random lastname than obtain a name for the list
        // Otherwise use the name that was passed through
        if(!randomLastName && existingLastName == "")
        {
            lastName = GetNode<CharacterDatabase>("/root/CharacterDatabase").GetRandomLastName();
        } else
        {
            lastName = existingLastName;
        }

        // Get a random country and state
        country = GetNode<CountryDatabase>("/root/CountryDatabase").GetRandomCountry();
        state = GetNode<CountryDatabase>("/root/CountryDatabase").GetState(country);

        CharacterDetails details = new CharacterDetails(firstName, lastName, 0, 0, sex, country, 
            state, 0, GetNode<GameController>("/root/GameController"));
            
        details.SetCharacterStats(GenerateCharacterStats(isBaby));

        GameController game = GetNode<GameController>("/root/GameController");
        EventDatabase eventDb = GetNode<EventDatabase>("/root/EventDatabase");
        
        GenerateBirthEvent(game, eventDb, ref details);

        // Return the new character details
        return details;
        
    }

    /// <summary>
    /// Generate a random character around the age passed in
    /// </summary>
    /// <param name="years">Age in years</param>
    /// <returns>New character details</returns>
    public CharacterDetails GenerateCharacterFromAge(int years)
    {
        CharacterDetails currentCharacter;

        if(years < 3)
            currentCharacter = GenerateRandomCharacter(true);
        else
            currentCharacter = GenerateRandomCharacter(false);

        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();
        int randomAge = rand.RandiRange(years - MinAgeModifier, years + MaxAgeModifier);
        currentCharacter.SetAge(rand.RandiRange(0, 12), randomAge);
        return currentCharacter;
    }


    /// <summary>
    /// Generate character stats using depending on whether they are a baby
    /// or not
    /// </summary>
    /// <param name="isBaby">If the character we are generating the stats for is a baby</param>
    /// <returns>New character stats</returns>
    private CharacterStats GenerateCharacterStats(bool isBaby = true)
    {
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();
        if(isBaby)
        {
            float smarts = rand.RandfRange(MinBabySmarts, MaxBabySmarts);
            float physical = rand.RandfRange(MinBabyPhysicalHealth, MaxBabyPhysicalHealth);
            float mental = rand.RandfRange(MinBabyMentalHealth, MaxBabyMentalHealth);
            float social = rand.RandfRange(MinBabySocial, MaxBabySocial);
            float looks = rand.RandfRange(MinBabyLooks, MaxBabyLooks);

            return new CharacterStats(smarts, physical, mental, social, looks);
        } else
        {
            float smarts = rand.RandfRange(MinAdultSmarts, MaxAdultSmarts);
            float physical = rand.RandfRange(MinAdultPhysicalHealth, MaxAdultPhysicalHealth);
            float mental = rand.RandfRange(MinAdultMentalHealth, MaxAdultMentalHealth);
            float social = rand.RandfRange(MinAdultSocial, MaxAdultSocial);
            float looks = rand.RandfRange(MinAdultLooks, MaxAdultLooks);

            if(smarts > 89)
                smarts = rand.RandfRange(MinAdultSmarts, MaxAdultSmarts);
            
            if(physical == 89)
                physical = rand.RandfRange(MinAdultPhysicalHealth, MaxAdultPhysicalHealth);

            if(mental > 89)
                mental = rand.RandfRange(MinAdultPhysicalHealth, MaxAdultPhysicalHealth);

            if(looks > 70)
                looks = rand.RandfRange(MinAdultLooks, MaxAdultLooks);

            return new CharacterStats(smarts, physical, mental, social, looks);

        }
    }

    public void GenerateBirthEvent(GameController game, EventDatabase eventDb, ref CharacterDetails character)
    {
        if(game != null && eventDb != null)
        {
            LifeEventLog log = CreateDateLog(game.CurrentEventID, game, character);
            character.AddLifeEvent(log);

            LifeEventLog birthEvent = eventDb.GetBirthEvent();
            birthEvent.IsBornEvent = true;
            birthEvent.Text = birthEvent.Text.Replace("(location)", $"{character.Country.Name}, {character.State}");
            birthEvent.Text = birthEvent.Text.Replace("(birthMonth)", game.GetMonthAsString());
            character.AddLifeEvent(birthEvent.Copy(game.CurrentEventID));

            game.CurrentEventID += 1;
        
        }
    }

    private LifeEventLog CreateDateLog(int id, GameController game, CharacterDetails character)
    {
        string monthAsString = game.GetMonthAsString();
        LifeEventLog date = new LifeEventLog($"{monthAsString} - {character.YearsOld} Years, {character.MonthsOld} Months", ELifeEventType.DATE, character.CharacterAgeCategory, character.Sex);
        return date.Copy(id);
    }
}