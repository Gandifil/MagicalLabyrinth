using System.Collections.Generic;
using System.Linq;
using MagicalLabyrinth.Screens;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;

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
        
        var abilities = MainGame.Instance.Content.Load<AbilityData[]>("abilities.json", new JsonContentLoader());
        if (_abilities.Count == abilities.Length)
            MainGame.Instance.ScreenManager
            .LoadScreen(new MessageScreen("Вы по праву можете собой гордиться, достигнув вершин развития в мире Волшебного Лабиринта.", MainGame.Screen));
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