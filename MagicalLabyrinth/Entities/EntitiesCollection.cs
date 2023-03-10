using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace MagicalLabyrinth.Entities;

public sealed class EntitiesCollection: IUpdate, IEnumerable<IEntity>
{
    private readonly List<IEntity> _entities = new(100);

    private readonly List<IEntity> _spawnEntities = new();

    public void Add(IEntity entity)
    {
        _spawnEntities.Add(entity);
    }

    public void Update(float dt)
    {
        if (_spawnEntities.Any())
        {
            _entities.AddRange(_spawnEntities);
            _spawnEntities.Clear();
        }
        foreach (var entity in _entities)
            entity.Update(dt);
        
        _entities.RemoveAll(x => !x.IsAlive);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var entity in _entities)
            entity.Draw(spriteBatch);
    }

    public IEnumerator<IEntity> GetEnumerator()
    {
        return _entities.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}