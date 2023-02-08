using MagicalLabyrinth.Entities;
using MagicalLabyrinth.Entities.Utils;

namespace MagicalLabyrinth.Utils;

public class Spawner
{
    private const float COOLDOWN = 5f;
    
    private readonly Timer _timer = new Timer();

    public Spawner()
    {
        _timer.Reset(COOLDOWN);
    }

    public void Update(float dt)
    {
        _timer.Update(dt);
        if (_timer.IsCompleted)
        {
            var screen = MainGame.Screen;
            screen.Spawn(new RedSkinEnemy(screen, 50));
            _timer.Reset(COOLDOWN);
        }
    }
}