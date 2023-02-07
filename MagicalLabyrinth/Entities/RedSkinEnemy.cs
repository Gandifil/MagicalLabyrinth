using System;
using MagicalLabyrinth.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MagicalLabyrinth.Entities;

public class RedSkinEnemy:Creature
{
    private readonly MainScreen _screen;
    
    public RedSkinEnemy(MainScreen screen, int x): base("adventurer-Sheet")
    {
        _screen = screen;
        _position.X = x;
    }

    public override void Update(GameTime gameTime)
    {        
        var animation = "idle";
        _isMoving = 0;
        if (NeedToLeft()  && !_isStriking)
        {
            SetLeftDirection();
            animation = "walk";
            _isMoving = 1;
        }
        
        if (NeedToRight() && !_isStriking)
        {
            SetRightDirection();
            animation = "walk";
            _isMoving = 1;
        }

        if (NeedToStrike() && !_isStriking)
            Strike();
        
        if (!_isStriking) 
            _sprite.Play(animation);
        
        
        base.Update(gameTime);
    }

    private void Strike(string name = "strike")
    {
        _sprite.Play(name + new Random().Next(1,4), () =>
        {
            _isStriking = false;
        });
        _isStriking = true;
    }

    private bool NeedToStrike()
    {
        return Math.Abs(_screen.Player.Position.X- Position.X) < 30
            && Math.Abs(_screen.Player.Position.Y- Position.Y) < 10;
    }

    private bool NeedToLeft()
    {
        return _screen.Player.Position.X < Position.X;
    }

    private bool NeedToRight()
    {
        return _screen.Player.Position.X > Position.X;
    }

    private bool _isStriking = false;
}