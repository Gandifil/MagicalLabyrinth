namespace MagicalLabyrinth;

/// <summary>
/// Defines a simple entity with logic linked on time.
/// </summary>
public interface IUpdate
{
    /// <summary>
    /// Process runtime logic of entity. Must be called every game tick.
    /// </summary>
    /// <param name="dt">Elapsed seconds</param>
    void Update(float dt);
}