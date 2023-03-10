using System;
using MagicalLabyrinth.Entities;
using MagicalLabyrinth.Entities.Utils;

namespace MagicalLabyrinth.Utils;

public class Spawner
{
    private const float COOLDOWN = 4f;
    
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
            if (((Creature)MainGame.Screen.Player).Body.Level > 5) Spawn();
        }

        if (MainGame.Screen.Entities.Count == 1)
        {
            Spawn();
            Spawn();
            Spawn();
        }
    }

    private void Spawn()
    {
        var screen = MainGame.Screen;
        var index = Random.Shared.Next(0, 10);
        
        IEntity newEntity = null;
        if (((Creature)MainGame.Screen.Player).Body.Level > 2 && index is >5 and <8 ) newEntity = new Golem(screen, screen.GetSpawnPoint());
        if (((Creature)MainGame.Screen.Player).Body.Level > 3 && index is >7 and <10 ) newEntity = new Necromancer(screen, screen.GetSpawnPoint());
        newEntity ??= new RedSkinEnemy(screen, screen.GetSpawnPoint());
        
        screen.Spawn(newEntity);
        _timer.Reset(((Creature)screen.Player).Body.Level > 7 ? COOLDOWN / 2: COOLDOWN);
    }
}