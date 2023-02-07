using System;
using MagicalLabyrinth.Sprites;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace MagicalLabyrinth.Entities.Utils;

public class Strike: IUpdate
{
    private readonly AnimatedSprite _sprite;
    private readonly Action _action;

    public bool IsCompleted { get; private set; }

    public Strike(AnimatedSprite sprite, Action action)
    {
        _sprite = sprite;
        _action = action;
    }

    public void Update(GameTime gameTime)
    {
        if (IsCompleted)
            return;
        
        if (_sprite.Progress > .5f)
        {
            _action?.Invoke();
            IsCompleted = true;
        }
    }
}