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

    private string _tags = string.Empty;

    public void AddAbility(AbilityData abilityData)
    {
        _abilities.Add(abilityData);
        BaseAttackPower += abilityData.BaseAttackPower;
        BaseAttackSpeed += abilityData.BaseAttackSpeed;
        SecondAttackPower += abilityData.SecondAttackPower;
        SecondCooldown += abilityData.SecondCooldown;

        if (!string.IsNullOrWhiteSpace(abilityData.Tags))
            _tags += $" {abilityData.Tags} ";
    }
    
    public bool Contain(string name)
    {
        return _abilities.Any(x => x.Name == name);
    }

    public bool HasTag(string name)
    {
        return _tags.Contains(name);
    }
}