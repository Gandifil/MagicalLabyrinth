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
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var walkSpeed = dt * 50;

        var animation = "idle";
        if (NeedToLeft()  && !_isStriking)
        {
            _sprite.Effect = SpriteEffects.FlipHorizontally;
            _position.X -= walkSpeed;
            animation = "walk";
        }
        
        if (NeedToRight() && !_isStriking)
        {
            _sprite.Effect = SpriteEffects.None;
            _position.X += walkSpeed;
            
            animation = "walk";
        }

        if (NeedToStrike() && !_isStriking)
            Strike();
        
        if (!_isStriking) 
            _sprite.Play(animation);
        _sprite.Update(dt);
    }

    private void Strike(string name = "strike")
    {
        _sprite.Play(name + new Random().Next(1,4).ToString(), () =>
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