namespace MagicalLabyrinth.Mechanics;

public interface IImpact
{ 
    Body Sender { get; }
    
    int Damage { get; }
}