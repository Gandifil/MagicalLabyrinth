using MagicalLabyrinth.Abilities;
using MagicalLabyrinth.Entities;

namespace MagicalLabyrinth.Mechanics;

public class EvolvingBody: Body
{
    public readonly ObservedParameter<int> Expirience;

    public readonly ObservedParameter<int> MaxExpirience;

    public int SkillPoints { get; private set; } = 1;
    
    public AbilityPack AbilityPack { get; private set; } = new AbilityPack();

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
        if (SkillPoints > 0 && !AbilityPack.Contain(abilityData.Name))
        {
            SkillPoints--;
            AbilityPack.AddAbility(abilityData);
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