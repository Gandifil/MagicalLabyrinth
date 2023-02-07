namespace MagicalLabyrinth.Sprites;

public interface IAnimationController
{
    string CurrentAnimationName { get; }

    float Progress { get; }
}