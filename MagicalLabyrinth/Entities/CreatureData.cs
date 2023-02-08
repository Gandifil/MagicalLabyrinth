namespace MagicalLabyrinth.Entities;

public class CreatureData
{
    public string SpriteSheetName { get; set; }

    public float Speed { get; set; }

    public float BaseAttack { get; set; }

    public float SecondAttack { get; set; }

    public float SecondCooldown { get; set; }

    public int Hp { get; set; }

    public int Level { get; set; }

    public float Jump { get; set; }
}