using System;
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
            Spawn();
            _timer.Reset(MainGame.Screen.Player.Level > 5 ? COOLDOWN / 2: COOLDOWN);
        }
    }

    private void Spawn()
    {
        var screen = MainGame.Screen;
        var index = Random.Shared.Next(0, 10);
        
        IEntity newEntity = null;
        if (MainGame.Screen.Player.Level > 2 && index is >5 and <8 ) newEntity = new Golem(screen, screen.GetSpawnPoint());
        if (MainGame.Screen.Player.Level > 3 && index is >7 and <10 ) newEntity = new Necromancer(screen, screen.GetSpawnPoint());
        newEntity ??= new RedSkinEnemy(screen, screen.GetSpawnPoint());
        
        screen.Spawn(newEntity);
    }
}