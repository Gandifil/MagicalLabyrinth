using MagicalLabyrinth.Abilities;
using MagicalLabyrinth.Entities;
using MagicalLabyrinth.Mechanics.Abilities;

namespace MagicalLabyrinth.Mechanics;

public class EvolvingBody: Body
{
    public readonly ObservedParameter<int> Expirience;

    public readonly ObservedParameter<int> MaxExpirience;

    public int SkillPoints { get; private set; } = 1;
    
    public virtual float this[AttributeType index] => (1 + Abilities.Factors[index])*СreatureData.Attributes[index];

    public AbilityCollection Abilities { get; private set; } = new ();

    public EvolvingBody(CreatureData creatureData): base(creatureData)
    {
        Expirience = new (0);
        MaxExpirience = new (8);
    }
    
    public void AddExpirience(int value)
    {
        Expirience.Value += value;
        while (Expirience >= MaxExpirience) LevelUp();
    }

    public void BuyAbility(AbilityData abilityData)
    {
        if (SkillPoints > 0 && !Abilities[abilityData.Name])
        {
            SkillPoints--;
            Abilities.Add(abilityData);
        }
    }

    private void LevelUp()
    {
        Expirience.Value -= MaxExpirience;
        MaxExpirience.Value *= 2;
        SkillPoints++;
        Level++;
    }
}