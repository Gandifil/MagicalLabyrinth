using System.Collections.Generic;
using System.Linq;

namespace MagicalLabyrinth.Abilities;

public class AbilityPack: IAbilityPack
{
    private readonly List<AbilityData> _abilities = new();

    public void AddAbility(AbilityData abilityData)
    {
        _abilities.Add(abilityData);
    }
    
    public bool Contain(string name)
    {
        return _abilities.Any(x => x.Name == name);
    }
}