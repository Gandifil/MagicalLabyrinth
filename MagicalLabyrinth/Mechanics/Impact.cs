namespace MagicalLabyrinth.Mechanics;

public class Impact: IImpact
{
    public Body Sender { get; }
    public int Damage { get; }

    public Impact(Body sender, int damage)
    {
        Sender = sender;
        Damage = damage;
    }
}