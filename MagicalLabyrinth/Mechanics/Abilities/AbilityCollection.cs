using System;
using System.Collections.Generic;
using System.Linq;
using MagicalLabyrinth.Screens;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;

namespace MagicalLabyrinth.Mechanics.Abilities;

public class AbilityCollection
{
    private readonly List<AbilityData> _abilities = new();
    private readonly HashSet<string> _abilitiesSet = new();
    private readonly Dictionary<AttributeType, float> _factors = 
        Enum.GetValues(typeof(AttributeType))
            .Cast<AttributeType>()
            .ToDictionary(x => x, _ => 0f);

    public bool this[string index] => _abilitiesSet.Contains(index);
    public IReadOnlyDictionary<AttributeType, float> Factors => _factors;

    public void Add(AbilityData abilityData)
    {
        _abilities.Add(abilityData);
        _abilitiesSet.Add(abilityData.Name);
        if (abilityData.AttributeType.HasValue)
            _factors[abilityData.AttributeType.Value] += abilityData.Factor;
        
        
        var abilities = MainGame.Instance.Content.Load<AbilityData[]>("abilities.json", new JsonContentLoader());
        if (_abilities.Count == abilities.Length)
            MainGame.Instance.ScreenManager
            .LoadScreen(new MessageScreen("Вы по праву можете собой гордиться, достигнув вершин развития в мире Волшебного Лабиринта.", MainGame.Screen));
    }
}