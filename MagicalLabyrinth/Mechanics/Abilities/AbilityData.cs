namespace MagicalLabyrinth.Mechanics.Abilities;

public class AbilityData
{
    public string Name { get; set; }
    
    public string Description { get; set; }

    public string SpriteSheet { get; set; }

    public AttributeType? AttributeType { get; set; }

    public float Factor { get; set; }
}