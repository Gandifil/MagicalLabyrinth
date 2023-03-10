using System;
using MagicalLabyrinth.Entities.Utils;
using MagicalLabyrinth.Mechanics;
using MagicalLabyrinth.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MagicalLabyrinth.Entities;

public class Necromancer: Creature
{
    private float HighBorder => .15f * Y_FLOOR_LEVEL;
    private float LowBorder => .55f * Y_FLOOR_LEVEL;

    private const float STRIKE_RANGE = 250;
    
    private readonly MainScreen _screen;

    private readonly Timer _strikeCooldown = new();
    
    public Necromancer(MainScreen screen, int X): base("Necromancer", X)
    {
        _screen = screen;
        _position.Y = Random.Shared.Next((int)HighBorder, (int)LowBorder);
    }

    public override RectangleF HitBox
    {
        get
        {
            var size = _sprite.TextureRegion.Size / 3;
            return new RectangleF((Position - _sprite.Origin + new Vector2(0, 30)).ToPoint() + size, size);
        }
    }

    private Vector2? _target;

    public override void Update(float dt)
    {        
        var animation = "idle";
        _isMoving = 0;

        if (_target != null)
        {
            if (_target.Value.X - Position.X > 0)
                SetRightDirection();
            else
                SetLeftDirection();
            animation = "walk";
            _isMoving = 1;

            var dy = _target.Value.Y - Position.Y;
            if (Math.Abs(dy) > 3f)
                _position.Y += Math.Sign(dy) * dt * _creatureData.Speed / 2;

            if ((_target.Value - Position).Length() < 30f)
                _target = null;
        }
        else if (Math.Abs(MainGame.Screen.Player.Position.X - Position.X) > STRIKE_RANGE)
        {
            _target = new Vector2(MainGame.Screen.Player.Position.X +
                                  Random.Shared.Next((int)-STRIKE_RANGE, (int)STRIKE_RANGE), 
                Random.Shared.Next((int)HighBorder, (int)LowBorder));
        }
        
        if (NeedToStrike())
            Strike();
        
        if (!_isStriking) 
            _sprite.Play(animation);

        _strikeCooldown.Update(dt);
        base.Update(dt);
    }

    private readonly SoundEffect _strikeSoundEffect = MainGame.Instance.Content.Load<SoundEffect>("sounds/Fire");
    private void Strike(string name = "strike1")
    {
        _sprite.Play(name, () =>
        {
            var shift = (MainGame.Screen.Player.Position - Position).NormalizedCopy() * 180;
            
            MainGame.Screen.Spawn(
                new Projectile(this, shift, creature => { creature.Body.Hurt(new Impact(this.Body, 10));}, "fireball")
                {
                    Position = Position,
                });
            _strikeSoundEffect.Play(1f, 1f, 1f);
            _isStriking = false;
        });
        _isStriking = true;
        _strikeCooldown.Reset(5f);
    }

    private bool NeedToStrike()
    {
        return _strikeCooldown.IsCompleted && _target == null;
    }

    private bool _isStriking = false;
}