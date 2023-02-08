using System.Collections.Generic;
using System.Linq;

namespace MagicalLabyrinth.Abilities;

public class AbilityPack: IAbilityPack
{
    private readonly List<AbilityData> _abilities = new();

    public float BaseAttackPower { get; private set; }

    public float BaseAttackSpeed { get; private set; }

    public float SecondAttackPower { get; private set; }

    public float SecondCooldown { get; private set; }

    public void AddAbility(AbilityData abilityData)
    {
        _abilities.Add(abilityData);
        BaseAttackPower += abilityData.BaseAttackPower;
        BaseAttackSpeed += abilityData.BaseAttackSpeed;
        SecondAttackPower += abilityData.SecondAttackPower;
        SecondCooldown += abilityData.SecondCooldown;
    }
    
    public bool Contain(string name)
    {
        return _abilities.Any(x => x.Name == name);
    }
}