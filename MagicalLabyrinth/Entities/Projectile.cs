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
    private readonly Action<Creature> _action;

    public float MaxRadius { get; set; } = 500;
    
    public Projectile(IEntity owner, Vector2 shift, Action<Creature> action, string contentName = "knife")
    {
        _owner = owner;
        _shift = shift;
        _action = action;
        SetupAnimatedSprite($"projectiles/{contentName}.sf");

        _rotation = shift.ToAngle() - 1.5708f;
    }

    public void CheckCollisions()
    {
        foreach (var entity in MainGame.Screen.Entities)
            if (entity is Creature creature)
                if (_owner is Player != entity is Player)
                    if (entity.HitBox.Contains(Position))
                    {
                        _action.Invoke(creature);
                        Die();
                    }
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        Position += dt * _shift;

        CheckCollisions();

        if (Position.Y > Y_FLOOR_LEVEL)
            Die();
        
        if ((_owner.Position - Position).Length() > MaxRadius)
            Die();
    }
}