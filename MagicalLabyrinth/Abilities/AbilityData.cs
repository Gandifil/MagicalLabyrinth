namespace MagicalLabyrinth.Abilities;

public class AbilityData
{
    public string Name { get; set; }
    
    public string Description { get; set; }

    public string SpriteSheet { get; set; }

    public float BaseAttackPower { get; set; }

    public float BaseAttackSpeed { get; set; }

    public float SecondAttackPower { get; set; }

    public float SecondCooldown { get; set; }
}