using System;
using MagicalLabyrinth.Entities.Utils;
using MagicalLabyrinth.Screens;
using Microsoft.Xna.Framework;
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

    private Vector2? _target;

    public override void Update(GameTime gameTime)
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
                _position.Y += Math.Sign(dy) * gameTime.GetElapsedSeconds() * _creatureData.Speed / 2;

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

        base.Update(gameTime);
    }

    private void Strike(string name = "strike1")
    {
        _sprite.Play(name, () =>
        {
            _isStriking = false;
        });
        _isStriking = true;
    }

    private bool NeedToStrike()
    {
        return _strikeCooldown.IsCompleted && _target == null;
    }

    private bool _isStriking = false;
}