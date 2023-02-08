using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;

namespace MagicalLabyrinth.Entities;

public class Projectile: Entity
{
    private readonly IEntity _owner;
    private readonly Vector2 _shift;

    public float MaxRadius { get; set; } = 500;
    
    public Projectile(IEntity owner, Vector2 shift)
    {
        _owner = owner;
        _shift = shift;
        SetupAnimatedSprite("projectiles/knife.sf");
        
        //_sprite.

        _rotation = shift.ToAngle() - 1.5708f;

        if (shift.X < 0)
            SetLeftDirection();
        else
            SetRightDirection();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        Position += gameTime.GetElapsedSeconds() * _shift;

        if (Position.Y > Y_FLOOR_LEVEL)
            Die();
        
        if ((_owner.Position - Position).Length() > MaxRadius)
            Die();
    }
}